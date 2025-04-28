using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaOrganizer.Storage.Local;

internal class DefaultCleanup : ICleanup
{
    private string sourceRoot;
    private HashSet<string> folders = [];

    private readonly ILogger logger;

    public DefaultCleanup(string sourceRoot, ILogger logger)
    {
        this.sourceRoot = sourceRoot ?? throw new ArgumentNullException(nameof(sourceRoot));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Track(string file)
    {
        folders.Add(Path.GetDirectoryName(file));
    }

    ///TODO: The current implementation is not optimal.
    /// Specifically, when going to remove empty folders, and if we're going to remove a parent folder anyway, there is no point in removing child folders first. Instead, removing parent only will bring the same value.
    public async Task RemoveEmptyFolders(IProgress<ProgressInfo> progress)
    {
        progress.Report(new ProgressInfo("Cleaning up empty folders", folders.Count, 0));

        // Expand the foldersList to include parent folder tree - up to the source root,
        // so we can delete all empty folders in the source root
        var expandedFoldersList = new HashSet<string>();
        foreach (var folder in folders)
        {
            var currentFolder = folder;
            while (currentFolder is not null && !PathsEqual(currentFolder, this.sourceRoot))
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
            await Task.Run(() =>
            {
                progress.Report(new ProgressInfo($"Deleting empty folder {emptyFolder}", emptyFolders.Count, i));
            });

            try
            {
                if (Directory.Exists(emptyFolder))
                {
                    Directory.Delete(emptyFolder);
                    this.logger.LogInformation($"Deleted empty folder: {emptyFolder}");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning($"Failed to delete empty folder {emptyFolder}. Reason: {ex.Message}");
            }

        }

        folders.Clear();
    }

    private static bool PathsEqual(string path1, string path2)
    {
        return string.Equals(path1, path2, StringComparison.InvariantCultureIgnoreCase);
    }
}
