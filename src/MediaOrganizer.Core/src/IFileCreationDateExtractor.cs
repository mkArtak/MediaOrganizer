using System;
using System.IO;

namespace MediaOrganizer.Core;

public interface IFileCreationDateExtractor
{
    public DateTime ExtractCreationDate(FileInfo file);
}
