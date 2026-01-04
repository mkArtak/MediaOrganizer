using MediaOrganizer.Core;
using System;
using System.IO;

namespace MediaOrganizer.Storage.Local;

public class FileInfoBasedCreationDateExtractor : IFileCreationDateExtractor
{
    public virtual DateTime ExtractCreationDate(FileInfo file)
    {
        return file.CreationTimeUtc > file.LastWriteTimeUtc ? file.LastWriteTimeUtc : file.CreationTimeUtc;
    }
}
