using MediaOrganizer.Core;
using System;

namespace MediaOrganizer.Storage.Local
{
    public class PhysicalFilesOrganizerFactory
    {
        private readonly PhysicalFileMover fileMover;

        private readonly PhysicalFileEnumerator enumerator;

        private readonly Mapper mapper;

        private readonly FilesOrganizerOptions options;

        public PhysicalFilesOrganizerFactory(FilesOrganizerOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.mapper = new PatternBasedPathMapper(options);
            this.fileMover = new PhysicalFileMover();
            this.enumerator = new PhysicalFileEnumerator();
        }

        public IFilesOrganizer Create()
        {
            return new PhysicalFileOrganizer(this.options, this.fileMover, this.enumerator, this.mapper);
        }
    }
}
