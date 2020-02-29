using MediaOrganizer.Core;
using MediaOrganizer.Local;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HomeMediaOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PhysicalFilesOrganizerFactory OrganizerFactory { get; }

        private bool isRunning = false;

        private CancellationTokenSource CancellationToken { get; set; }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set
            {
                if (value != this.isRunning)
                {
                    this.isRunning = value;

                    this.OnStateChanged();
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

        private bool DeleteOnMove
        {
            get => this.chkDeleteOnMove.IsChecked.Value;
        }

        public MainWindow()
        {
            InitializeComponent();

            this.OrganizerFactory = new PhysicalFilesOrganizerFactory();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.IsRunning = !this.IsRunning))
            {
                this.CancellationToken.Cancel();
                return;
            }

            this.CancellationToken?.Dispose();
            this.CancellationToken = new CancellationTokenSource();

            try
            {
                await OrganizeFilesByDatesAndTypes(this.DestinationDirectory, this.SourceDirectory, this.DeleteOnMove);
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        private async Task OrganizeFilesByDatesAndTypes(string mediaRoot, string sourceRoot, bool deleteOnMove)
        {

            if (!Directory.Exists(sourceRoot))
            {
                throw new DirectoryNotFoundException("Source not found");
            }

            var organizer = this.OrganizerFactory.Create();
            var options = new FilesOrganizerOptions { RemoveSource = deleteOnMove, SourceRoot = sourceRoot, DestinationRoot = mediaRoot };
            await organizer.OrganizeAsync(options, this.CancellationToken.Token);
        }
    }
}
