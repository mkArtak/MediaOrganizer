using MediaOrganizer.Core;
using MediaOrganizer.Services;
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

    private readonly IOrganizerFactory organizerFactory;

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

    public FileOrganizerViewModel(IAppStateManager appStateManager, IOrganizerFactory organizerFactory, ILogger<FileOrganizerViewModel> logger) : base()
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.organizerFactory = organizerFactory ?? throw new ArgumentNullException(nameof(organizerFactory));
        this.appStateManager = appStateManager ?? throw new ArgumentNullException(nameof(appStateManager));

        this.progress = new Progress<ProgressInfo>(ReportProgress);
        _loadTask = Task.Run(async () => await InitializeAsync());

        this.OrganizeFilesCommand = new DelegateCommand(() =>
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
        if (progressInfo.TotalFiles != 0)
            this.TotalProgress = progressInfo.CurrentFileIndex * 100 / progressInfo.TotalFiles;
    }
}
