using MediaOrganizer.Core;
using System;
using System.IO;

namespace MediaOrganizer.Storage.Local;

public class FileInfoBasedCreationDateExtractor : IFileCreationDateExtractor
{
    public virtual DateTime ExtractCreationDate(FileInfo file)
    {
        return file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime;
    }
}
