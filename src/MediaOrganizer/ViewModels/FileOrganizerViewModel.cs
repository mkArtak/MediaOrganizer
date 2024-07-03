using MediaOrganizer.Storage.Local;
using Microsoft.Extensions.Logging;
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
        private string currentFile;

        private readonly PhysicalFilesOrganizerFactory organizerFactory;

        private CancellationTokenSource cancellationToken;
        private Task organizingTask;
        private readonly Progress<string> progress;
        private ILogger<FileOrganizerViewModel> logger;

        public FileOrganizerOptionsViewModel Options { get; } = new FileOrganizerOptionsViewModel() { VideoSubfolderName = "Movies", PhotosSubfolderName = "Photos" };

        public DelegateCommand OrganizeFilesCommand { get; private set; }

        public bool IsRunning
        {
            get => this.isRunning;
            set { SetProperty(ref this.isRunning, value); }
        }

        public string CurrentFile
        {
            get => this.currentFile;
            set => SetProperty(ref this.currentFile, value);
        }

        public FileOrganizerViewModel() : base()
        {
            this.OrganizeFilesCommand = new DelegateCommand(async () => await Organize(), () => !this.IsRunning);

            this.logger = ApplicationLogging.CreateLogger<FileOrganizerViewModel>();
            this.organizerFactory = new PhysicalFilesOrganizerFactory(this.logger);
            this.progress = new Progress<string>(ReportProgress);
        }

        private async Task Organize()
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
                this.CurrentFile = null;

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
            await organizer.OrganizeAsync(progress, this.cancellationToken.Token);
        }

        private void ReportProgress(string sourceFilePath)
        {
            this.logger.LogInformation(sourceFilePath);
            this.CurrentFile = sourceFilePath;
        }

        internal static class ApplicationLogging
        {
            public static ILoggerFactory LogFactory { get; } = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                // Clear Microsoft's default providers (like event logs and others)
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
