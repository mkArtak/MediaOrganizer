using MediaOrganizer.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Storage.Local
{
    internal sealed class PhysicalFileOrganizer : IFilesOrganizer
    {
        private IFileMover FileMover { get; }

        private IFileEnumerator FileEnumerator { get; }

        private IMapper Mapper { get; }

        private FilesOrganizerOptions Options { get; }

        public PhysicalFileOrganizer(FilesOrganizerOptions options, IFileMover mover, IFileEnumerator enumerator, IMapper mapper)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.FileMover = mover ?? throw new ArgumentNullException(nameof(mover));
            this.FileEnumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            this.Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task OrganizeAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var moverOptions = new FileMoverOptions { RemoveSourceAfterMove = this.Options.RemoveSource };
            CreateDestinationIfNotExist(this.Options.DestinationRoot);

            foreach (string file in this.FileEnumerator.GetFilesAsync(this.Options.SourceRoot))
            {
                if (token.IsCancellationRequested)
                    return;

                var destinationPath = this.Mapper.GetDestination(file);
                if (destinationPath == null)
                {
                    // This file should not be moved
                    continue;
                }

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
    }
}
