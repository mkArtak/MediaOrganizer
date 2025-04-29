// Create a command-line utility that will utilize MediaOrganizer.Core and MediaOrganizer.Storage.Local to organize media files.
// This utility should expose all input via parameters for users to pass in.

using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

var sourceOption = new Option<DirectoryInfo>("--source", "The source directory containing media files that need to be organized.")
{
    IsRequired = true
};

var destinationOption = new Option<DirectoryInfo>("--destination", "The destination directory to organize media files under.")
{
    IsRequired = true
};

var removeSourceOption = new Option<bool>("--remove-source", () => true, "Remove source files after moving.")
{
    IsRequired = false
};

var skipExistingOption = new Option<bool>("--skip-existing", () => true, "Skip files if they already exist in the destination (comparison is done based on the filename.")
{
    IsRequired = false
};
var videoSubfolderNameOption = new Option<string>("--video-subfolder-name", () => FilesOrganizerOptions.DefaultVideoSubfolderName, "The name of the subfolder to organize video files under.")
{
    IsRequired = false,
};
var photosSubfolderNameOption = new Option<string>("--photos-subfolder-name", () => FilesOrganizerOptions.DefaultPhotosSubfolderName, "The name of the subfolder to organize photo files under.")
{
    IsRequired = false,
};
var imageFileFormatPatternsOption = new Option<string[]>("--image-file-format-patterns", () => FilesOrganizerOptions.DefaultPhotoFileFormatPatterns, "The file formats to consider as images.")
{
    IsRequired = false,
};
var videoFileFormatPatternsOption = new Option<string[]>("--video-file-format-patterns", () => FilesOrganizerOptions.DefaultVideoFileFormatPatterns, "The file formats to consider as videos.")
{
    IsRequired = false,
};

RootCommand rootCommand = new RootCommand("Organizes media files based on the provided parameters.")
{
    sourceOption,
    destinationOption,
    removeSourceOption,
    skipExistingOption,
    videoSubfolderNameOption,
    photosSubfolderNameOption,
    imageFileFormatPatternsOption,
    videoFileFormatPatternsOption
};

rootCommand.SetHandler(async (source, destination, removeSource, skipExisting, videoSubfolderName, photosSubfolderName, imageFileFormatPatterns, videoFileFormatPatterns) => await OrganizeAsync(source.FullName, destination.FullName, removeSource, skipExisting, videoSubfolderName, photosSubfolderName, imageFileFormatPatterns, videoFileFormatPatterns), sourceOption, destinationOption, removeSourceOption, skipExistingOption, videoSubfolderNameOption, photosSubfolderNameOption, imageFileFormatPatternsOption, videoFileFormatPatternsOption);

await rootCommand.InvokeAsync(args);

async Task OrganizeAsync(string source, string destination, bool removeSource, bool skipExisting, string videoSubfolderName, string photosSubfolderName, string[] imageFileFormatPatterns, string[] videoFileFormatPatterns)
{
    var options = new FilesOrganizerOptions
    {
        SourceRoot = source,
        DestinationRoot = destination,
        RemoveSource = removeSource,
        SkipExistingFiles = skipExisting,
        VideoSubfolderName = videoSubfolderName,
        PhotosSubfolderName = photosSubfolderName,
        ImageFileFormatPatterns = imageFileFormatPatterns,
        VideoFileFormatPatterns = videoFileFormatPatterns
    };
    var fileMoverOptions = new FileMoverOptions
    {
        RemoveSourceAfterMove = options.RemoveSource,
        SkipIfFileExists = options.SkipExistingFiles,
        DeleteEmptyFolders = options.DeleteEmptyFolders
    };


    var sc = new ServiceCollection();
    sc.AddLogging();
    sc.AddLocalStorageServices();
    var sp = sc.BuildServiceProvider();

    var factory = sp.GetRequiredService<IOrganizerFactory>();
    var organizer = factory.Create(options);
    await organizer.OrganizeAsync(new Progress<ProgressInfo>(), CancellationToken.None);
}