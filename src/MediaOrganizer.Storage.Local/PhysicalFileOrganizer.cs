﻿using MediaOrganizer.Core;
using System;
using System.Collections.Generic;
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

        public async Task OrganizeAsync(IProgress<string> progress, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var moverOptions = new FileMoverOptions { RemoveSourceAfterMove = this.Options.RemoveSource };
            CreateDestinationIfNotExist(this.Options.DestinationRoot);

            IList<Task> moveTasks = new List<Task>();

            foreach (string file in this.FileEnumerator.GetFilesAsync(this.Options.SourceRoot))
            {
                if (token.IsCancellationRequested)
                    return;

                var destinationPath = this.Mapper.GetDestination(file);
                if (destinationPath == null)
                {
                    // This file should not be moved
                    // TODO: Add logging here.
                    continue;
                }

                await this.FileMover.MoveAsync(moverOptions, file, destinationPath);
                progress.Report(file);

                //moveTasks.Add(this.FileMover.MoveAsync(moverOptions, file, destinationPath));
            }

            await Task.WhenAll(moveTasks);
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
