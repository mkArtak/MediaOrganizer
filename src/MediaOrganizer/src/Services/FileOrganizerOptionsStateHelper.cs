using MediaOrganizer.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaOrganizer.Services;

internal static class FileOrganizerOptionsStateHelper
{
    public static void SaveFileOrganizerOptionsState(IAppStateManager stateManager, FilesOrganizerOptions options)
    {
        stateManager.UpdateState(nameof(FilesOrganizerOptions.SourceRoot), options.SourceRoot);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.DestinationRoot), options.DestinationRoot);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.RemoveSource), options.RemoveSource);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.SkipExistingFiles), options.SkipExistingFiles);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.DestinationPattern), options.DestinationPattern);
        stateManager.UpdateState(nameof(FilesOrganizerOptions.DeleteEmptyFolders), options.DeleteEmptyFolders);
        SaveMediaCategories(stateManager, options.MediaCategories);
    }

    public static async Task<FilesOrganizerOptions> GetFileOrganizerOptionsAsync(IAppStateManager stateManager)
    {
        var result = new FilesOrganizerOptions
        {
            SourceRoot = await GetSourceRoot(stateManager),
            DestinationRoot = await GetDestinationRoot(stateManager),
            RemoveSource = await GetRemoveSource(stateManager),
            SkipExistingFiles = await GetSkipExistingFiles(stateManager),
            DestinationPattern = await GetDestinationPattern(stateManager),
            DeleteEmptyFolders = await GetDeleteEmptyFolders(stateManager),
        };

        var mediaCategories = await GetMediaCategories(stateManager);
        result.MediaCategories.AddRange(mediaCategories);
        return result;
    }

    private static async Task<IEnumerable<MediaCategory>> GetMediaCategories(IAppStateManager stateManager)
    {
        var storedMediaCategoryNamesString = await stateManager.GetState<string>(FilesOrganizerOptions.WellKnownMediaCategoriesKey);
        var storedMediaCategoryNames = storedMediaCategoryNamesString?.Split(IAppStateManager.ArrayElementsSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (storedMediaCategoryNames is null || storedMediaCategoryNames.Length == 0)
        {
            // If no media categories are configured, return the default ones
            return [WellKnownMediaCategories.Movies, WellKnownMediaCategories.Photos];
        }

        var result = new List<MediaCategory>();
        foreach (var categoryName in storedMediaCategoryNames)
        {
            var category = await GetMediaCategoryOptions(categoryName, stateManager);
            if (category is not null)
            {
                result.Add(category);
            }
        }

        return result;
    }

    private static async Task<MediaCategory> GetMediaCategoryOptions(string categoryName, IAppStateManager stateManager)
    {
        MediaCategory result;

        try
        {
            result = new MediaCategory();
            result.CategoryName = categoryName;
            result.CategoryRoot = await stateManager.GetState<string>($"{categoryName}.{nameof(MediaCategory.CategoryRoot)}");

            var extensions = await stateManager.GetState<string>($"{categoryName}.{nameof(MediaCategory.FileExtensions)}");
            if (extensions is null || extensions.Length == 0)
                return null;

            result.FileExtensions = extensions.Split(IAppStateManager.ArrayElementsSeparator);
        }
        catch
        {
            result = null;
        }

        return result;
    }

    private static void SaveMediaCategories(IAppStateManager stateManager, IEnumerable<MediaCategory> categories)
    {
        var categoryNames = new List<string>();
        foreach (var category in categories)
        {
            SaveMediaCategoryOptions(category, stateManager);
            categoryNames.Add(category.CategoryName);
        }

        stateManager.UpdateState(FilesOrganizerOptions.WellKnownMediaCategoriesKey, String.Join(IAppStateManager.ArrayElementsSeparator, categoryNames));
    }

    private static void SaveMediaCategoryOptions(MediaCategory category, IAppStateManager stateManager)
    {
        stateManager.UpdateState($"{category.CategoryName}.{nameof(MediaCategory.CategoryRoot)}", category.CategoryRoot);
        stateManager.UpdateState($"{category.CategoryName}.{nameof(MediaCategory.FileExtensions)}", string.Join(IAppStateManager.ArrayElementsSeparator, category.FileExtensions));
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
