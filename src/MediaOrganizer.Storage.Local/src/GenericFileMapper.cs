using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local.Extensions;
using System;
using System.IO;
using System.Linq;

namespace MediaOrganizer.Storage.Local;

public class GenericFileMapper : IMapper
{
    private readonly string[] patterns;
    private readonly string destinationRoot;
    private static readonly string[] monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    private readonly string destinationFolderPattern;

    public GenericFileMapper(string[] patterns, string destinationRoot, string destinationPattern)
    {
        this.patterns = patterns ?? throw new ArgumentNullException(nameof(patterns));
        this.destinationRoot = destinationRoot ?? throw new ArgumentNullException(nameof(destinationRoot));
        this.destinationFolderPattern = destinationPattern ?? throw new ArgumentNullException(nameof(destinationPattern));
    }

    public bool TryGetDestination(string path, out string destination)
    {
        if (!IsFileMappable(path))
        {
            destination = null;
            return false;
        }

        FileInfo info = new FileInfo(path);
        var dateTaken = info.CreationTimeUtc > info.LastWriteTimeUtc ? info.LastWriteTimeUtc : info.CreationTimeUtc;
        string relativePath = GetRelativeDestinationFolder(dateTaken);
        string destinationDirectory = Path.Combine(this.destinationRoot, relativePath);

        FileSystemExtensions.CreateDirectoryIfNotExists(destinationDirectory);

        destination = Path.Combine(destinationDirectory, info.Name);
        return true;
    }

    internal string GetRelativeDestinationFolder(DateTime dateTaken)
    {
        return destinationFolderPattern
                    .Replace("{Year}", dateTaken.Year.ToString(), StringComparison.OrdinalIgnoreCase)
                    .Replace("{Month}", dateTaken.Month.ToString("D2"), StringComparison.OrdinalIgnoreCase)
                    .Replace("{MonthName}", monthNames[dateTaken.Month - 1], StringComparison.OrdinalIgnoreCase)
                    .Replace("{Day}", dateTaken.Day.ToString("D2"), StringComparison.OrdinalIgnoreCase);
    }

    internal bool IsFileMappable(string filename)
    {
        string extension = Path.GetExtension(filename).ToLowerInvariant();
        return this.patterns.Contains(extension);
    }
}
