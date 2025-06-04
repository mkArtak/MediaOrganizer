using MediaOrganizer.Core;
using MediaOrganizer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaOrganizer.Tests;

public class FileOrganizerOptionsStateHelperTests
{
    private class FakeAppStateManager : IAppStateManager
    {
        public readonly Dictionary<string, string> State = new();

        public Task BeginLoadState() => Task.CompletedTask;
        public Task SaveStateAsync() => Task.CompletedTask;

        public Task<T> GetState<T>(string key)
        {
            if (State.TryGetValue(key, out var value))
            {
                object result = null;
                if (typeof(T) == typeof(string))
                    result = value;
                else if (typeof(T) == typeof(bool))
                    result = bool.Parse(value);
                else if (typeof(T) == typeof(string[]))
                    result = value.Split(IAppStateManager.ArrayElementsSeparator, StringSplitOptions.RemoveEmptyEntries);
                return Task.FromResult((T)result);
            }
            return Task.FromResult(default(T));
        }

        public void UpdateState<T>(string key, T state)
        {
            if (state is string[] array)
                State[key] = string.Join(IAppStateManager.ArrayElementsSeparator, array);
            else
                State[key] = state?.ToString();
        }
    }

    [Fact]
    public async Task SaveAndLoadOptions_WithMultipleCategories()
    {
        // Arrange
        var options = new FilesOrganizerOptions
        {
            SourceRoot = "src",
            DestinationRoot = "dest",
            RemoveSource = true,
            SkipExistingFiles = false,
            DestinationPattern = "{Year}",
            DeleteEmptyFolders = true
        };
        options.MediaCategories.Add(new MediaCategory { CategoryName = "photos", CategoryRoot = "pics", FileExtensions = [".jpg"] });
        options.MediaCategories.Add(new MediaCategory { CategoryName = "videos", CategoryRoot = "vids", FileExtensions = [".mp4"] });

        var stateManager = new FakeAppStateManager();

        // Act
        FileOrganizerOptionsStateHelper.SaveFileOrganizerOptionsState(stateManager, options);
        var loaded = await FileOrganizerOptionsStateHelper.GetFileOrganizerOptionsAsync(stateManager);

        // Assert
        Assert.Equal(options.SourceRoot, loaded.SourceRoot);
        Assert.Equal(options.DestinationRoot, loaded.DestinationRoot);
        Assert.Equal(options.RemoveSource, loaded.RemoveSource);
        Assert.Equal(options.SkipExistingFiles, loaded.SkipExistingFiles);
        Assert.Equal(options.DestinationPattern, loaded.DestinationPattern);
        Assert.Equal(options.DeleteEmptyFolders, loaded.DeleteEmptyFolders);
        Assert.Equal(2, loaded.MediaCategories.Count);
        Assert.Contains(loaded.MediaCategories, c => c.CategoryName == "photos" && c.CategoryRoot == "pics" && c.FileExtensions.SequenceEqual([".jpg"]));
        Assert.Contains(loaded.MediaCategories, c => c.CategoryName == "videos" && c.CategoryRoot == "vids" && c.FileExtensions.SequenceEqual([".mp4"]));
    }
}
