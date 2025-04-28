using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Storage.Local;

internal sealed class PhysicalFileOrganizer : IFilesOrganizer
{
    private readonly Action<string> createDirectoryIfNotExistHandler;

    private IFileMover FileMover { get; }

    private IFileEnumerator FileEnumerator { get; }

    private IMapper Mapper { get; }

    private FilesOrganizerOptions Options { get; }

    private FileMoverOptions MoverOptions { get; }

    private ILogger Logger { get; }

    private ICleanup CleanupHandler { get; set; }

    public PhysicalFileOrganizer(FilesOrganizerOptions options, IFileMover mover, IFileEnumerator enumerator, IMapper mapper, ICleanup cleanupHandler, ILogger logger) : this(options, mover, enumerator, mapper, cleanupHandler, FileSystemExtensions.CreateDirectoryIfNotExists, logger)
    {
    }

    internal PhysicalFileOrganizer(FilesOrganizerOptions options, IFileMover mover, IFileEnumerator enumerator, IMapper mapper, ICleanup cleanupHandler, Action<string> createDirectoryIfNotExistHandler, ILogger logger)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.MoverOptions = new FileMoverOptions
        {
            RemoveSourceAfterMove = options.RemoveSource,
            SkipIfFileExists = options.SkipExistingFiles,
            DeleteEmptyFolders = options.DeleteEmptyFolders
        };

        this.FileMover = mover ?? throw new ArgumentNullException(nameof(mover));
        this.FileEnumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        this.Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.createDirectoryIfNotExistHandler = createDirectoryIfNotExistHandler ?? throw new ArgumentNullException(nameof(createDirectoryIfNotExistHandler));
        this.CleanupHandler = cleanupHandler ?? throw new ArgumentNullException(nameof(cleanupHandler));
    }

    public async Task OrganizeAsync(IProgress<ProgressInfo> progress, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;

        this.Logger.LogInformation($"Preparing to move files from {this.Options.SourceRoot} to {this.Options.DestinationRoot}");

        createDirectoryIfNotExistHandler(this.Options.SourceRoot);

        var counter = 0;
        var files = this.FileEnumerator.GetFiles(this.Options.SourceRoot);
        var totalFiles = files.Count();
        if (totalFiles > 0)
        {
            bool removeEmptyFolders = this.Options.RemoveSource && this.Options.DeleteEmptyFolders;

            foreach (string file in files)
            {
                if (token.IsCancellationRequested)
                    break;

                if (!this.Mapper.TryGetDestination(file, out var destinationPath))
                {
                    this.Logger.LogWarning($"Skipping file as it didn't match any of the extension patterns: {file}");
                    continue;
                }

                if (removeEmptyFolders)
                    this.CleanupHandler.Track(file);

                await this.FileMover.MoveAsync(this.MoverOptions, file, destinationPath);

                counter++;
                progress.Report(new ProgressInfo(file, totalFiles, counter));
            }

            if (removeEmptyFolders)
                await this.CleanupHandler.RemoveEmptyFolders(progress);
        }
        else
        {
            this.Logger.LogWarning($"No files found in {this.Options.SourceRoot} to move.");
        }

        this.Logger.LogInformation("Finished moving files");
    }
}
