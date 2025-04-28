using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
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

    public PhysicalFileOrganizer(FilesOrganizerOptions options, IFileMover mover, IFileEnumerator enumerator, IMapper mapper, ILogger logger) : this(options, mover, enumerator, mapper, FileSystemExtensions.CreateDirectoryIfNotExists, logger)
    {
    }

    internal PhysicalFileOrganizer(FilesOrganizerOptions options, IFileMover mover, IFileEnumerator enumerator, IMapper mapper, Action<string> createDirectoryIfNotExistHandler, ILogger logger)
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
    }

    public async Task OrganizeAsync(IProgress<ProgressInfo> progress, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;

        this.Logger.LogInformation($"Preparing to move files from {this.Options.SourceRoot} to {this.Options.DestinationRoot}");

        createDirectoryIfNotExistHandler(this.Options.SourceRoot);

        var foldersList = new HashSet<string>();
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

                if (removeEmptyFolders)
                {
                    // Build the list of all folders that will potentially be deleted
                    // This will be more optimal instead of checking each time a file is moved from a folder, as each folder can have multiple files
                    foldersList.Add(Path.GetDirectoryName(file));
                }

                if (!this.Mapper.TryGetDestination(file, out var destinationPath))
                {
                    this.Logger.LogWarning($"Skipping file as it didn't match any of the extension patterns: {file}");
                    continue;
                }

                await this.FileMover.MoveAsync(this.MoverOptions, file, destinationPath);

                counter++;
                progress.Report(new ProgressInfo(file, totalFiles, counter));
            }

            if (removeEmptyFolders)
                await RemoveEmptyFoldersAsync(progress, foldersList);
        }
        else
        {
            this.Logger.LogWarning($"No files found in {this.Options.SourceRoot} to move.");
        }

        this.Logger.LogInformation("Finished moving files");
    }

    private async Task RemoveEmptyFoldersAsync(IProgress<ProgressInfo> progress, HashSet<string> foldersList)
    {
        progress.Report(new ProgressInfo("Cleaning up empty folders", foldersList.Count, 0));

        // Expand the foldersList to include parent folder tree - up to the source root,
        // so we can delete all empty folders in the source root
        var expandedFoldersList = new HashSet<string>();
        foreach (var folder in foldersList)
        {
            var currentFolder = folder;
            while (currentFolder is not null && !PathsEqual(currentFolder, this.Options.SourceRoot))
            {
                expandedFoldersList.Add(currentFolder);
                currentFolder = Path.GetDirectoryName(currentFolder);
            }
        }

        var emptyFolders = new List<string>();
        foreach (var folder in expandedFoldersList)
        {
            if (Directory.EnumerateFiles(folder).Count() == 0)
            {
                emptyFolders.Add(folder);
            }
        }

        progress.Report(new ProgressInfo("Cleaning up empty folders", emptyFolders.Count, 0));

        // Sort the folders by length in descending order, so we can delete the deepest folders first
        emptyFolders.Sort((a, b) =>
        {
            var aLength = a.Length;
            var bLength = b.Length;
            return aLength == bLength ? 0 : aLength < bLength ? 1 : -1;
        });

        for (var i = 0; i < emptyFolders.Count; i++)
        {
            var emptyFolder = emptyFolders[i];
            progress.Report(new ProgressInfo($"Deleting empty folder {emptyFolder}", emptyFolders.Count, i));

            await Task.Run(() =>
            {
                try
                {
                    if (Directory.Exists(emptyFolder))
                    {
                        Directory.Delete(emptyFolder);
                        this.Logger.LogInformation($"Deleted empty folder: {emptyFolder}");
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogWarning($"Failed to delete empty folder {emptyFolder}. Reason: {ex.Message}");
                }
            });
        }
    }

    private static bool PathsEqual(string path1, string path2)
    {
        return string.Equals(path1, path2, StringComparison.InvariantCultureIgnoreCase);
    }
}
