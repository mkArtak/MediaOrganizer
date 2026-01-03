using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace MediaOrganizer.CLI;

internal class OrganizeCommandHandler : RootCommand
{
    private readonly Option<DirectoryInfo> sourceOption = new Option<DirectoryInfo>("--source")
    {
        Description = "The source directory containing media files that need to be organized.",
        Required = true
    };

    private readonly Option<DirectoryInfo> destinationOption = new Option<DirectoryInfo>("--destination")
    {
        Description = "The destination directory to organize media files under.",
        Required = true
    };

    private readonly Option<bool> removeSourceOption = new Option<bool>("--remove-source")
    {
        Description = "Remove source files after moving.",
        DefaultValueFactory = ar => true,
        Required = false
    };

    private readonly Option<bool> skipExistingOption = new Option<bool>("--skip-existing")
    {
        Description = "Skip files if they already exist in the destination (comparison is done based on the filename).",
        DefaultValueFactory = ar => true,
        Required = false
    };

    private readonly Option<string[]> mediaExtensionsOption = new Option<string[]>("--media-extensions")
    {
        Description = "The file extensions that will be be organized under the specified destination directory.",
        DefaultValueFactory = ar => Array.Empty<string>(),
        Required = true,
    };

    private readonly Option<string> destinationPatternOption = new Option<string>("--destination-pattern")
    {
        Description = "The pattern used to create subfolders in the destination directory. Default is '{Year}/{MonthName}/{Year}-{Month}-{Day}'.",
        DefaultValueFactory = ar => FilesOrganizerOptions.DefaultDestinationPattern,
        Required = false
    };

    private readonly Option<bool> deleteEmptyFoldersOption = new Option<bool>("--delete-empty-folders")
    {
        Description = "Delete empty folders in the source directory after organizing.",
        DefaultValueFactory = ar => true,
        Required = false
    };

    private readonly IOrganizerFactory _organizerFactory;
    private readonly ILogger _logger;

    public OrganizeCommandHandler(IOrganizerFactory organizerFactory, ILogger<OrganizeCommandHandler> logger)
    {
        _organizerFactory = organizerFactory ?? throw new ArgumentNullException(nameof(organizerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Description = "Organize media files into subfolders based on specified options";

        Options.Add(sourceOption);
        Options.Add(destinationOption);
        Options.Add(removeSourceOption);
        Options.Add(skipExistingOption);
        Options.Add(mediaExtensionsOption);
        Options.Add(destinationPatternOption);
        Options.Add(deleteEmptyFoldersOption);

        this.SetAction(parseResult => InvokeAsync(parseResult));
    }

    private async Task<int> InvokeAsync(ParseResult parseResult)
    {
        var source = parseResult.GetRequiredValue(sourceOption).FullName;
        var destination = parseResult.GetRequiredValue(destinationOption).FullName;
        var removeSource = parseResult.GetValue(removeSourceOption);
        var skipExisting = parseResult.GetValue(skipExistingOption);
        var imageFileFormatPatterns = parseResult.GetValue(mediaExtensionsOption);
        var destinationPattern = parseResult.GetValue(destinationPatternOption);
        var deleteEmptyFolders = parseResult.GetValue(deleteEmptyFoldersOption);

        var options = new FilesOrganizerOptions
        {
            SourceRoot = source,
            DestinationRoot = destination,
            RemoveSource = removeSource,
            SkipExistingFiles = skipExisting,
            DestinationPattern = destinationPattern,
            DeleteEmptyFolders = deleteEmptyFolders,
        };

        var extensions = parseResult.GetValue(mediaExtensionsOption);
        if (extensions!.Length == 1 && extensions[0].IndexOf(',') > 0)
        {
            extensions = extensions[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        options.MediaCategories.Add(new MediaCategory
        {
            CategoryName = "custom",
            CategoryRoot = "",
            FileExtensions = extensions
        });

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