using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaOrganizer.Core
{
    internal sealed class PhysicalFileOrganizer : IFilesOrganizer
    {
        private const string VideoSubfolderName = "Movies";
        private const string PhotosSubfolderName = "Photos";

        private static readonly string[] VideoFileFormatPatterns = new[] { ".mov", ".mp4", ".avi", ".3gp", ".mpg" };

        private static readonly string[] ImageFileFormatPatterns = new[] { ".jpg", ".jpeg" };


        private IFileMover FileMover { get; }

        private IFileEnumerator FileEnumerator { get; }

        public PhysicalFileOrganizer(IFileMover mover, IFileEnumerator enumerator)
        {
            this.FileMover = mover ?? throw new ArgumentNullException(nameof(mover));
            this.FileEnumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public async Task OrganizeAsync(FilesOrganizerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var moverOptions = new FileMoverOptions { RemoveSourceAfterMove = options.RemoveSource };
            CreateDestinationIfNotExist(options.DestinationRoot);

            foreach (string file in Directory.EnumerateFiles(options.SourceRoot, "*", SearchOption.AllDirectories))
            {
                FileType type = GetFileType(file);
                if (type == FileType.Unknown)
                {
                    continue;
                }

                string destinationPath = GetDestinationDirectoryForFile(options.DestinationRoot, file, type);
                await this.FileMover.MoveAsync(moverOptions, file, destinationPath);
            }
        }

        private void CreateDestinationIfNotExist(string destinationRoot)
        {
            if (!Directory.Exists(destinationRoot))
            {
                Directory.CreateDirectory(destinationRoot);
            }
        }

        private string GetDestinationDirectoryForFile(string root, string path, FileType fileType)
        {
            FileInfo info = new FileInfo(path);
            DateTime dateTaken = info.CreationTimeUtc > info.LastWriteTimeUtc ? info.LastWriteTimeUtc : info.CreationTimeUtc;
            string typeSubfolder;
            switch (fileType)
            {
                case FileType.Image:
                    typeSubfolder = PhotosSubfolderName;
                    break;

                case FileType.Movie:
                    typeSubfolder = VideoSubfolderName;
                    break;

                default:
                    throw new ApplicationException("Unexpected file type");
            }

            string directory = Path.Combine(root, typeSubfolder, dateTaken.Year.ToString(), dateTaken.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, Path.GetFileName(path));
        }

        private static FileType GetFileType(string filename)
        {
            FileType result = FileType.Unknown;
            string extension = Path.GetExtension(filename).ToLowerInvariant();
            if (ImageFileFormatPatterns.Contains(extension))
            {
                result = FileType.Image;
            }
            else if (VideoFileFormatPatterns.Contains(extension))
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
