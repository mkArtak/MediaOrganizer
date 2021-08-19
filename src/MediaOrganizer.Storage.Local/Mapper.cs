using MediaOrganizer.Core;
using System;
using System.IO;
using System.Linq;

namespace MediaOrganizer.Storage.Local
{
    public class Mapper : IMapper
    {
        private readonly FilesOrganizerOptions options;

        public Mapper(FilesOrganizerOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string GetDestination(string path)
        {
            FileType type = GetFileType(options, path);
            if (type == FileType.Unknown)
            {
                return null;
            }

            FileInfo info = new FileInfo(path);
            var dateTaken = info.CreationTimeUtc > info.LastWriteTimeUtc ? info.LastWriteTimeUtc : info.CreationTimeUtc;
            string mediaTypeFolder = type switch
            {
                FileType.Image => this.options.PhotosSubfolderName,
                FileType.Movie => this.options.VideoSubfolderName,
                _ => throw new ApplicationException("Unexpected file type")
            };
            string relativePath = GetDestinationFolder(dateTaken);
            string directory = Path.Combine(this.options.DestinationRoot, mediaTypeFolder, relativePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, Path.GetFileName(path));
        }

        protected virtual string GetDestinationFolder(DateTime dateTaken)
        {
            return Path.Combine(dateTaken.Year.ToString(), dateTaken.ToString("yyyy-MM-dd"));
        }

        private static FileType GetFileType(FilesOrganizerOptions options, string filename)
        {
            string extension = Path.GetExtension(filename).ToLowerInvariant();

            FileType result;
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
