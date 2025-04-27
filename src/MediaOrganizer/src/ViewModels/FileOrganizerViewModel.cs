using MediaOrganizer.Core;
using MediaOrganizer.Services;
using MediaOrganizer.Storage.Local;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.ViewModels;

public class FileOrganizerViewModel : BindableBase
{
    public bool isRunning;
    private string currentFile;
    private string statusMessage;

    private readonly PhysicalFilesOrganizerFactory organizerFactory;

    private CancellationTokenSource cancellationToken;
    private Task organizerTask;
    private readonly Progress<ProgressInfo> progress;
    private readonly IAppStateManager appStateManager;
    private readonly Task _loadTask;
    private ILogger<FileOrganizerViewModel> logger;
    private FileOrganizerOptionsViewModel options;
    private int totalProgress = 0;

    public FileOrganizerOptionsViewModel Options
    {
        get => options;
        set => SetProperty(ref this.options, value);
    }

    public DelegateCommand OrganizeFilesCommand { get; init; }

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

    public int TotalProgress
    {
        get => this.totalProgress;
        set => SetProperty(ref this.totalProgress, value);
    }

    public FileOrganizerViewModel() : base()
    {
        this.OrganizeFilesCommand = new DelegateCommand(async () =>
        {
            if (this.IsRunning)
            {
                this.cancellationToken.Cancel();
            }
            else
            {
                this.organizerTask = Organize();
            }
        });

        this.logger = ApplicationLogging.CreateLogger<FileOrganizerViewModel>();
        this.organizerFactory = new PhysicalFilesOrganizerFactory(this.logger);
        this.progress = new Progress<ProgressInfo>(ReportProgress);

        this.appStateManager = AppStateManager.Instance;
        _loadTask = Task.Run(async () => await InitializeAsync());
    }

    private async Task InitializeAsync()
    {
        var options = await ReloadOptionsFromState();
        this.Options = new FileOrganizerOptionsViewModel(options);
    }

    private async Task<FilesOrganizerOptions> ReloadOptionsFromState()
    {
        await this.appStateManager.BeginLoadState();

        return await FileOrganizerOptionsStateHelper.GetFileOrganizerOptionsAsync(this.appStateManager);
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
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred during file organization.");
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

        var options = this.Options.GetOptions();

        FileOrganizerOptionsStateHelper.SaveFileOrganizerOptionsState(this.appStateManager, options);
        var organizer = this.organizerFactory.Create(options);
        await organizer.OrganizeAsync(progress, this.cancellationToken.Token);
    }

    private void ReportProgress(ProgressInfo progressInfo)
    {
        this.logger.LogInformation(progressInfo.FileName);
        this.CurrentFile = progressInfo.FileName;
        this.TotalProgress = progressInfo.CurrentFileIndex * 100 / progressInfo.TotalFiles;
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
