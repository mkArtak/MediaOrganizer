using System;
using System.Threading.Tasks;

namespace MediaOrganizer.Core;

public interface ICleanup
{
    /// <summary>
    /// Tracks a file that has been processed as part of organizing media for later cleanup.
    /// </summary>
    /// <param name="file">The path to the file that was processed</param>
    void Track(string file);

    /// <summary>
    /// Removes all empty folders that remain post-organizing operation, that are within the specified source root.
    /// </summary>
    /// <param name="progress">Used to report progress</param>
    Task RemoveEmptyFolders(IProgress<ProgressInfo> progress);
}
