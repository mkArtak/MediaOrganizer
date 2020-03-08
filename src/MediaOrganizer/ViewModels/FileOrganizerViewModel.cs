using MediaOrganizer.Core;
using MediaOrganizer.Local;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HomeMediaOrganizer.ViewModels
{
    public class FileOrganizerViewModel : BindableBase
    {
        private string destinatioNDirectory;
        private string sourceDirectory;
        private bool deleteOnMove;
        public bool isRunning;

        private CancellationTokenSource cancellationToken;
        private PhysicalFilesOrganizerFactory organizerFactory;


        public DelegateCommand OrganizeFilesCommand { get; private set; }


        public bool IsRunning
        {
            get => this.isRunning;
            set { SetProperty(ref this.isRunning, value); }
        }

        public string SourceDirectory
        {
            get => this.sourceDirectory;
            set { SetProperty(ref this.sourceDirectory, value); }
        }

        public string DestinationDirectory
        {
            get => this.destinatioNDirectory;
            set { SetProperty(ref this.destinatioNDirectory, value); }
        }

        public bool DeleteOnMove
        {

            get => this.deleteOnMove;
            set { SetProperty(ref this.deleteOnMove, value); }
        }

        public FileOrganizerViewModel() : base()
        {
            this.organizerFactory = new PhysicalFilesOrganizerFactory();

            this.OrganizeFilesCommand = new DelegateCommand(Organize, () => !this.IsRunning);
        }

        private async void Organize()
        {
            this.IsRunning = true;

            try
            {
                this.cancellationToken?.Cancel();
                this.cancellationToken?.Dispose();
                this.cancellationToken = new CancellationTokenSource();

                await OrganizeFilesByDatesAndTypes();
            }
            finally
            {
                this.IsRunning = false;

                this.cancellationToken.Dispose();
                this.cancellationToken = null;
            }
        }

        private async Task OrganizeFilesByDatesAndTypes()
        {

            if (!Directory.Exists(this.SourceDirectory))
            {
                throw new DirectoryNotFoundException("Source not found");
            }

            var organizer = this.organizerFactory.Create();
            var options = new FilesOrganizerOptions { RemoveSource = this.DeleteOnMove, SourceRoot = this.SourceDirectory, DestinationRoot = this.DestinationDirectory };
            await organizer.OrganizeAsync(options, this.cancellationToken.Token);
        }
    }
}
