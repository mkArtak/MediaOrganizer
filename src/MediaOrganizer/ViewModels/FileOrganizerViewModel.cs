using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.ViewModels
{
    public class FileOrganizerViewModel : BindableBase
    {
        public bool isRunning;

        private CancellationTokenSource cancellationToken;
        private PhysicalFilesOrganizerFactory organizerFactory;

        public FileOrganizerOptionsViewModel Options { get; } = new FileOrganizerOptionsViewModel() { VideoSubfolderName = "Movies", PhotosSubfolderName = "Photos" };

        public DelegateCommand OrganizeFilesCommand { get; private set; }

        public bool IsRunning
        {
            get => this.isRunning;
            set { SetProperty(ref this.isRunning, value); }
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
            if (!Directory.Exists(this.Options.SourceRoot))
            {
                throw new DirectoryNotFoundException("Source not found");
            }

            var organizer = this.organizerFactory.Create();
            await organizer.OrganizeAsync(this.Options.GetOptions(), this.cancellationToken.Token);
        }
    }
}
