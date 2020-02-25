using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Core
{
    internal sealed class PhysicalFileOrganizer : IFilesOrganizer
    {
        private IFileMover FileMover { get; }

        private IFileEnumerator FileEnumerator { get; }

        public PhysicalFileOrganizer(IFileMover mover, IFileEnumerator enumerator)
        {
            this.FileMover = mover ?? throw new ArgumentNullException(nameof(mover));
            this.FileEnumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public async Task OrganizeAsync(FilesOrganizerOptions options, CancellationToken token)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (token.IsCancellationRequested)
                return;

            var moverOptions = new FileMoverOptions { RemoveSourceAfterMove = options.RemoveSource };
            CreateDestinationIfNotExist(options.DestinationRoot);

            foreach (string file in this.FileEnumerator.GetFilesAsync(options.SourceRoot))
            {
                if (token.IsCancellationRequested)
                    return;

                FileType type = GetFileType(options, file);
                if (type == FileType.Unknown)
                {
                    continue;
                }

                string destinationPath = GetDestinationDirectoryForFile(options, file, type);
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

        private string GetDestinationDirectoryForFile(FilesOrganizerOptions options, string path, FileType fileType)
        {
            FileInfo info = new FileInfo(path);
            var dateTaken = info.CreationTimeUtc > info.LastWriteTimeUtc ? info.LastWriteTimeUtc : info.CreationTimeUtc;
            string typeSubfolder;
            switch (fileType)
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
