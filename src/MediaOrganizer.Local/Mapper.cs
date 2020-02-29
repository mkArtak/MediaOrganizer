using MediaOrganizer.Core;
using System;
using System.IO;
using System.Linq;

namespace MediaOrganizer.Local
{
    public class Mapper : IMapper
    {
        public Mapper()
        {

        }

        public string GetDestination(FilesOrganizerOptions options, string path)
        {
            FileType type = GetFileType(options, path);
            if (type == FileType.Unknown)
            {
                return null;
            }

            FileInfo info = new FileInfo(path);
            var dateTaken = info.CreationTimeUtc > info.LastWriteTimeUtc ? info.LastWriteTimeUtc : info.CreationTimeUtc;
            string typeSubfolder;
            switch (type)
            {
                case FileType.Image:
                    typeSubfolder = options.PhotosSubfolderName;
                    break;

                case FileType.Movie:
                    typeSubfolder = options.VideoSubfolderName;
                    break;

                default:
                    throw new ApplicationException("Unexpected file type");
            }

            string directory = Path.Combine(options.DestinationRoot, typeSubfolder, dateTaken.Year.ToString(), dateTaken.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, Path.GetFileName(path));
        }

        private static FileType GetFileType(FilesOrganizerOptions options, string filename)
        {
            FileType result = FileType.Unknown;
            string extension = Path.GetExtension(filename).ToLowerInvariant();
            if (options.ImageFileFormatPatterns.Contains(extension))
            {
                result = FileType.Image;
            }
            else if (options.VideoFileFormatPatterns.Contains(extension))
            {
                result = FileType.Movie;
            }
            else
            {
                result = FileType.Unknown;
            }

            return result;
        }

        private enum FileType
        {
            Image,
            Movie,
            Unknown
        }
    }
}
