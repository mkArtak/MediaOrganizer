using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using System;

namespace MediaOrganizer.Storage.Local
{
    public class PhysicalFilesOrganizerFactory
    {
        private ILogger logger;

        public PhysicalFilesOrganizerFactory(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public IFilesOrganizer Create(FilesOrganizerOptions options)
        {
            var mapper = new PatternBasedPathMapper(options);
            var fileMover = new PhysicalFileMover(this.logger);
            var enumerator = new PhysicalFileEnumerator();

            return new PhysicalFileOrganizer(options, fileMover, enumerator, mapper, this.logger);
        }
    }
}
