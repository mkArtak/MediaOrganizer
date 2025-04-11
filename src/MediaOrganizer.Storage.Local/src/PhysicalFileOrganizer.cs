using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Storage.Local;

internal sealed class PhysicalFileOrganizer : IFilesOrganizer
{
    private IFileMover FileMover { get; }

    private IFileEnumerator FileEnumerator { get; }

    private IMapper Mapper { get; }

    private FilesOrganizerOptions Options { get; }

    private FileMoverOptions MoverOptions { get; }

    private ILogger Logger { get; }

    public PhysicalFileOrganizer(FilesOrganizerOptions options, IFileMover mover, IFileEnumerator enumerator, IMapper mapper, ILogger logger)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.MoverOptions = new FileMoverOptions
        {
            RemoveSourceAfterMove = options.RemoveSource,
            SkipIfFileExists = options.SkipExistingFiles
        };

        this.FileMover = mover ?? throw new ArgumentNullException(nameof(mover));
        this.FileEnumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        this.Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OrganizeAsync(IProgress<ProgressInfo> progress, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;

        this.Logger.LogInformation($"Preparing to move files from {this.Options.SourceRoot} to {this.Options.DestinationRoot}");

        CreateDestinationIfNotExist(this.Options.DestinationRoot);

        var counter = 0;
        var files = this.FileEnumerator.GetFiles(this.Options.SourceRoot);
        var totalFiles = files.Count();

        foreach (string file in files)
        {
            if (token.IsCancellationRequested)
                break;

            if (!this.Mapper.TryGetDestination(file, out var destinationPath))
            {
                this.Logger.LogWarning($"Skipping file as it didn't match any of the extension patterns: {file}");
                continue;
            }

            await this.FileMover.MoveAsync(this.MoverOptions, file, destinationPath);
            counter++;
            progress.Report(new ProgressInfo(file, totalFiles, counter));
        }

        this.Logger.LogInformation("Finished moving files");
    }

    private void CreateDestinationIfNotExist(string destinationRoot)
    {
        if (!Directory.Exists(destinationRoot))
        {
            Directory.CreateDirectory(destinationRoot);
        }
    }
}
