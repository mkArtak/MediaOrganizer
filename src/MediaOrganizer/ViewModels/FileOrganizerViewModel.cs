using MediaOrganizer.Storage.Local;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.ViewModels
{
    public class FileOrganizerViewModel : BindableBase
    {
        public bool isRunning;

        private readonly PhysicalFilesOrganizerFactory organizerFactory;

        private CancellationTokenSource cancellationToken;

        public FileOrganizerOptionsViewModel Options { get; } = new FileOrganizerOptionsViewModel() { VideoSubfolderName = "Movies", PhotosSubfolderName = "Photos" };

        public DelegateCommand OrganizeFilesCommand { get; private set; }

        public bool IsRunning
        {
            get => this.isRunning;
            set { SetProperty(ref this.isRunning, value); }
        }

        public FileOrganizerViewModel() : base()
        {
            this.OrganizeFilesCommand = new DelegateCommand(Organize, () => !this.IsRunning);

            this.organizerFactory = new PhysicalFilesOrganizerFactory(ApplicationLogging.CreateLogger<FileOrganizerViewModel>());
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

            var organizer = this.organizerFactory.Create(this.Options.GetOptions());
            await organizer.OrganizeAsync(this.cancellationToken.Token);
        }

        internal static class ApplicationLogging
        {
            public static ILoggerFactory LogFactory { get; } = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                // Clear Microsoft's default providers (like eventlogs and others)
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                }).AddDebug().SetMinimumLevel(LogLevel.Information);
            });

            public static ILogger<T> CreateLogger<T>() => LogFactory.CreateLogger<T>();
        }
    }
}
