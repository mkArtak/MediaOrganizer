using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace MediaOrganizer.CLI;

internal class OrganizeCommandHandler : RootCommand, ICommandHandler
{
    private readonly Option<DirectoryInfo> sourceOption = new Option<DirectoryInfo>("--source", "The source directory containing media files that need to be organized.")
    {
        IsRequired = true
    };

    private readonly Option<DirectoryInfo> destinationOption = new Option<DirectoryInfo>("--destination", "The destination directory to organize media files under.")
    {
        IsRequired = true
    };

    private readonly Option<bool> removeSourceOption = new Option<bool>("--remove-source", () => true, "Remove source files after moving.")
    {
        IsRequired = false
    };

    private readonly Option<bool> skipExistingOption = new Option<bool>("--skip-existing", () => true, "Skip files if they already exist in the destination (comparison is done based on the filename.")
    {
        IsRequired = false
    };
    private readonly Option<string> videoSubfolderNameOption = new Option<string>("--video-subfolder-name", () => FilesOrganizerOptions.DefaultVideoSubfolderName, "The name of the subfolder to organize video files under.")
    {
        IsRequired = false,
    };
    private readonly Option<string> photosSubfolderNameOption = new Option<string>("--photos-subfolder-name", () => FilesOrganizerOptions.DefaultPhotosSubfolderName, "The name of the subfolder to organize photo files under.")
    {
        IsRequired = false,
    };
    private readonly Option<string[]> imageFileFormatPatternsOption = new Option<string[]>("--image-file-format-patterns", () => FilesOrganizerOptions.DefaultPhotoFileFormatPatterns, "The file formats to consider as images.")
    {
        IsRequired = false,
    };
    private readonly Option<string[]> videoFileFormatPatternsOption = new Option<string[]>("--video-file-format-patterns", () => FilesOrganizerOptions.DefaultVideoFileFormatPatterns, "The file formats to consider as videos.")
    {
        IsRequired = false,
    };

    private readonly Option<string> destinationPatternOption = new Option<string>("--destination-pattern", () => FilesOrganizerOptions.DefaultDestinationPattern, "The pattern to use for organizing files in the destination folder.")
    {
        IsRequired = false
    };
    private readonly Option<bool> deleteEmptyFoldersOption = new Option<bool>("--delete-empty-folders", () => true, "Delete empty folders after moving files.")
    {
        IsRequired = false
    };
    private readonly IOrganizerFactory _organizerFactory;
    private readonly ILogger _logger;

    public OrganizeCommandHandler(IOrganizerFactory organizerFactory, ILogger<OrganizeCommandHandler> logger)
    {
        _organizerFactory = organizerFactory ?? throw new ArgumentNullException(nameof(organizerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        AddOption(sourceOption);
        AddOption(destinationOption);
        AddOption(removeSourceOption);
        AddOption(skipExistingOption);
        AddOption(videoSubfolderNameOption);
        AddOption(photosSubfolderNameOption);
        AddOption(imageFileFormatPatternsOption);
        AddOption(videoFileFormatPatternsOption);
        AddOption(destinationPatternOption);
        AddOption(deleteEmptyFoldersOption);
        Description = "Organize media files into subfolders based on specified options";

        this.Handler = this;
    }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var source = context.ParseResult.GetValueForOption(sourceOption)!.FullName;
        var destination = context.ParseResult.GetValueForOption(destinationOption)!.FullName;
        var removeSource = context.ParseResult.GetValueForOption(removeSourceOption);
        var skipExisting = context.ParseResult.GetValueForOption(skipExistingOption);
        var videoSubfolderName = context.ParseResult.GetValueForOption(videoSubfolderNameOption);
        var photosSubfolderName = context.ParseResult.GetValueForOption(photosSubfolderNameOption);
        var imageFileFormatPatterns = context.ParseResult.GetValueForOption(imageFileFormatPatternsOption);
        var videoFileFormatPatterns = context.ParseResult.GetValueForOption(videoFileFormatPatternsOption);
        var destinationPattern = context.ParseResult.GetValueForOption(destinationPatternOption);
        var deleteEmptyFolders = context.ParseResult.GetValueForOption(deleteEmptyFoldersOption);

        var options = new FilesOrganizerOptions
        {
            SourceRoot = source,
            DestinationRoot = destination,
            RemoveSource = removeSource,
            SkipExistingFiles = skipExisting,
            VideoSubfolderName = videoSubfolderName,
            PhotosSubfolderName = photosSubfolderName,
            ImageFileFormatPatterns = imageFileFormatPatterns,
            VideoFileFormatPatterns = videoFileFormatPatterns,
            DestinationPattern = destinationPattern,
            DeleteEmptyFolders = deleteEmptyFolders
        };

        var organizer = _organizerFactory.Create(options);
        try
        {
            var progressReporter = new Progress<ProgressInfo>(info =>
            {
                Console.WriteLine($"Progress: {info.FileName}");
            });

            await organizer.OrganizeAsync(progressReporter, CancellationToken.None);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}