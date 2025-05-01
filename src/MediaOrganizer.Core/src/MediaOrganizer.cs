using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Core;

public class MediaOrganizer : IFilesOrganizer
{
    private readonly Action<string> _createDirectoryIfNotExistHandler;

    private readonly IFileMover _fileMover;
    private readonly IFileEnumerator _fileEnumerator;
    private readonly IMapper _mapper;
    private readonly FilesOrganizerOptions _options;
    private readonly FileMoverOptions _moverOptions;
    private readonly ILogger _logger;
    private readonly ICleanup _cleanupHandler;

    public MediaOrganizer(FilesOrganizerOptions options, IFileMover mover, IFileEnumerator enumerator, IMapper mapper, ICleanup cleanupHandler, Action<string> createDirectoryIfNotExistHandler, ILogger logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _moverOptions = new FileMoverOptions
        {
            RemoveSourceAfterMove = options.RemoveSource,
            SkipIfFileExists = options.SkipExistingFiles,
            DeleteEmptyFolders = options.DeleteEmptyFolders
        };

        _fileMover = mover ?? throw new ArgumentNullException(nameof(mover));
        _fileEnumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _createDirectoryIfNotExistHandler = createDirectoryIfNotExistHandler ?? throw new ArgumentNullException(nameof(createDirectoryIfNotExistHandler));
        _cleanupHandler = cleanupHandler ?? throw new ArgumentNullException(nameof(cleanupHandler));
    }

    public async Task OrganizeAsync(IProgress<ProgressInfo> progress, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;

        _logger.LogInformation($"Preparing to move files from {_options.SourceRoot} to {_options.DestinationRoot}");

        _createDirectoryIfNotExistHandler(_options.SourceRoot);

        var counter = 0;
        var files = _fileEnumerator.GetFiles(_options.SourceRoot);
        var totalFiles = files.Count();
        if (totalFiles > 0)
        {
            bool removeEmptyFolders = _options.RemoveSource && _options.DeleteEmptyFolders;

            foreach (string file in files)
            {
                if (token.IsCancellationRequested)
                    break;

                if (!_mapper.TryGetDestination(file, out var destinationPath))
                {
                    _logger.LogWarning($"Skipping file as it didn't match any of the extension patterns: {file}");
                    continue;
                }

                if (removeEmptyFolders)
                    _cleanupHandler.Track(file);

                counter++;

                ProgressInfo progressToReport;
                try
                {
                    await _fileMover.MoveAsync(_moverOptions, file, destinationPath);
                    progressToReport = new ProgressInfo(file, totalFiles, counter);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to organize file {file}. Reason: {ex.Message}");
                    progressToReport = new ProgressInfo(file, totalFiles, counter, false);
                }

                progress.Report(progressToReport);
            }

            if (removeEmptyFolders)
            {
                _logger.LogInformation("Preparing to remove empty folders");
                try
                {
                    await _cleanupHandler.RemoveEmptyFolders(progress);
                    _logger.LogInformation("Successfully removed empty folders");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Removal of empty folders failed with error {ex.Message}");
                }
            }

            _logger.LogInformation("Finished organizing files");
        }
        else
        {
            _logger.LogWarning($"No files found in {_options.SourceRoot} to move.");
        }
    }
}
