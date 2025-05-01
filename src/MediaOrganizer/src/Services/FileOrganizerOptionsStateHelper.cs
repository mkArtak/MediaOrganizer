using MediaOrganizer.Core;
using System.Threading.Tasks;

namespace MediaOrganizer.Services;

internal static class FileOrganizerOptionsStateHelper
{
    public static void SaveFileOrganizerOptionsState(IAppStateManager stateManager, FilesOrganizerOptions options)
    {
        stateManager.UpdateState(nameof(FilesOrganizerOptions.VideoFileFormatPatterns), string.Join(IAppStateManager.ExtensionsSeparator, options.VideoFileFormatPatterns));
        stateManager.UpdateState(nameof(FilesOrganizerOptions.ImageFileFormatPatterns), string.Join(IAppStateManager.ExtensionsSeparator, options.ImageFileFormatPatterns));
        stateManager.UpdateState(nameof(FilesOrganizerOptions.VideoSubfolderName), options.VideoSubfolderName);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.PhotosSubfolderName), options.PhotosSubfolderName);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.SourceRoot), options.SourceRoot);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.DestinationRoot), options.DestinationRoot);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.RemoveSource), options.RemoveSource);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.SkipExistingFiles), options.SkipExistingFiles);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.DestinationPattern), options.DestinationPattern);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.DeleteEmptyFolders), options.DeleteEmptyFolders);
    }

    public static async Task<FilesOrganizerOptions> GetFileOrganizerOptionsAsync(IAppStateManager stateManager)
    {
        return new FilesOrganizerOptions
        {
            VideoFileFormatPatterns = await GetVideoFileFormatPatterns(stateManager),
            ImageFileFormatPatterns = await GetImageFileFormatPatterns(stateManager),
            VideoSubfolderName = await GetVideoSubfolderName(stateManager),
            PhotosSubfolderName = await GetPhotosSubfolderName(stateManager),
            SourceRoot = await GetSourceRoot(stateManager),
            DestinationRoot = await GetDestinationRoot(stateManager),
            RemoveSource = await GetRemoveSource(stateManager),
            SkipExistingFiles = await GetSkipExistingFiles(stateManager),
            DestinationPattern = await GetDestinationPattern(stateManager),
            DeleteEmptyFolders = await GetDeleteEmptyFolders(stateManager)
        };
    }

    private static async Task<string[]> GetVideoFileFormatPatterns(IAppStateManager stateManager)
    {
        return await stateManager.GetStateOrDefault<string[]>(
            nameof(FilesOrganizerOptions.VideoFileFormatPatterns),
            FilesOrganizerOptions.DefaultVideoFileFormatPatterns,
            val => val.Split(IAppStateManager.ExtensionsSeparator));
    }

    private static async Task<string[]> GetImageFileFormatPatterns(IAppStateManager stateManager)
    {
        return await stateManager.GetStateOrDefault<string[]>(
            nameof(FilesOrganizerOptions.ImageFileFormatPatterns),
            FilesOrganizerOptions.DefaultPhotoFileFormatPatterns,
            val => val.Split(IAppStateManager.ExtensionsSeparator));
    }

    private static async Task<string> GetVideoSubfolderName(IAppStateManager stateManager)
    {
        return await stateManager.GetStateOrDefault<string>(
            nameof(FilesOrganizerOptions.VideoSubfolderName),
             FilesOrganizerOptions.DefaultVideoSubfolderName);
    }

    public static async Task<string> GetPhotosSubfolderName(IAppStateManager stateManager)
    {
        return await stateManager.GetStateOrDefault<string>(
            nameof(FilesOrganizerOptions.PhotosSubfolderName),
            FilesOrganizerOptions.DefaultPhotosSubfolderName);
    }

    private static Task<string> GetSourceRoot(IAppStateManager stateManager) => stateManager.GetState<string>(nameof(FilesOrganizerOptions.SourceRoot));

    private static Task<string> GetDestinationRoot(IAppStateManager stateManager) => stateManager.GetState<string>(nameof(FilesOrganizerOptions.DestinationRoot));


    private static Task<bool> GetRemoveSource(IAppStateManager stateManager) => stateManager.GetState<bool>(nameof(FilesOrganizerOptions.RemoveSource));


    private static Task<bool> GetSkipExistingFiles(IAppStateManager stateManager) => stateManager.GetState<bool>(nameof(FilesOrganizerOptions.SkipExistingFiles));

    private static Task<bool> GetDeleteEmptyFolders(IAppStateManager stateManager) => stateManager.GetState<bool>(nameof(FilesOrganizerOptions.DeleteEmptyFolders));

    private static Task<string> GetDestinationPattern(IAppStateManager stateManager)
    {
        return stateManager.GetStateOrDefault<string>(
            nameof(FilesOrganizerOptions.DestinationPattern),
            FilesOrganizerOptions.DefaultDestinationPattern);
    }
}
