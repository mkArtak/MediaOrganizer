using System.IO;

namespace MediaOrganizer.Storage.Local.Extensions;

internal static class FileSystemExtensions
{
    public static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
