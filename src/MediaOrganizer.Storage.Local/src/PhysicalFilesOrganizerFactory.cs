using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace MediaOrganizer.Storage.Local;

public class PhysicalFilesOrganizerFactory : IOrganizerFactory
{
    private readonly IFileMover _mover;
    private readonly ILogger _logger;
    private readonly IFileEnumerator _fileEnumerator;

    public PhysicalFilesOrganizerFactory(IFileMover mover, IFileEnumerator fileEnumerator, ILogger<PhysicalFilesOrganizerFactory> logger)
    {
        _logger = logger ?? throw new ArgumentException(nameof(logger));
        _mover = mover ?? throw new ArgumentNullException(nameof(mover));
        _fileEnumerator = fileEnumerator ?? throw new ArgumentNullException(nameof(fileEnumerator));
    }

    public IFilesOrganizer Create(FilesOrganizerOptions options)
    {
        var mapper = new MediaFileMapper(options);
        var cleanupHandler = new DefaultCleanup(options.SourceRoot, _logger);

        return new Core.MediaOrganizer(options, _mover, _fileEnumerator, mapper, cleanupHandler, FileSystemExtensions.CreateDirectoryIfNotExists, _logger);
    }
}
