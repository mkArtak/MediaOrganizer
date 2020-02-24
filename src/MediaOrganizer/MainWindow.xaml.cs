using MediaOrganizer.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeMediaOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FilesOrganizerFactory OrganizerFactory { get; }

        private bool isRunning = false;

        public bool IsRunning
        {
            get { return this.isRunning; }
            set
            {
                if (value != this.isRunning)
                {
                    this.isRunning = value;
                }
            }
        }

        private void OnStateChanged()
        {
            this.txtDestination.IsEnabled = !this.IsRunning;
            this.txtSource.IsEnabled = !this.IsRunning;
            this.btnStart.Content = this.IsRunning ? "Stop" : "Start";
            this.prgProgress.Visibility = this.IsRunning ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private string SourceDirectory
        {
            get
            {
                return this.txtSource.Text;
            }
        }

        private string DestinationDirectory
        {
            get
            {
                return this.txtDestination.Text;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.OrganizerFactory = new FilesOrganizerFactory();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(this.SourceDirectory))
            {
                throw new DirectoryNotFoundException("Source not found");
            }

            if (this.IsRunning = !this.IsRunning)
            {
                this.OnStateChanged();
            }
            else
            {
                return;
            }

            await OrganizeFilesByDatesAndTypes(this.DestinationDirectory, this.SourceDirectory);

            this.IsRunning = false;
            this.OnStateChanged();
        }

        private async Task OrganizeFilesByDatesAndTypes(string mediaRoot, string sourceRoot)
        {
            var organizer = this.OrganizerFactory.Create();
            var options = new FilesOrganizerOptions { RemoveSource = true, SourceRoot = sourceRoot, DestinationRoot = mediaRoot };
            await organizer.OrganizeAsync(options);
        }
    }
}
