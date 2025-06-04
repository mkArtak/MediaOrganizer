using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local;
using System;
using System.IO;
using Xunit;

namespace MediaOrganizer.Storage.Local.Tests;

public class MediaFileMapperTests
{
    [Fact]
    public void TryGetDestination_UsesAllCategories()
    {
        RunInTempDirectory(root =>
        {
            var sourceRoot = Path.Combine(root, "source");
            var destRoot = Path.Combine(root, "dest");

            var options = new FilesOrganizerOptions
            {
                DestinationRoot = destRoot,
                DestinationPattern = "{Year}"
            };
            options.MediaCategories.Add(new MediaCategory { CategoryName = "Photos", CategoryRoot = "pics", FileExtensions = [".jpg"] });
            options.MediaCategories.Add(new MediaCategory { CategoryName = "Videos", CategoryRoot = "vids", FileExtensions = [".mp4"] });

            var mapper = new MediaFileMapper(options);

            var photo = CreateTempFile(sourceRoot, "image.jpg");
            File.SetCreationTime(photo, new DateTime(2024, 1, 1));

            var result = mapper.TryGetDestination(photo, out var destination);
            Assert.True(result);
            Assert.Equal(Path.Combine(destRoot, "pics", "2024", "image.jpg"), destination);

            var video = CreateTempFile(sourceRoot, "movie.mp4");
            File.SetCreationTime(video, new DateTime(2025, 2, 2));
            result = mapper.TryGetDestination(video, out destination);
            Assert.True(result);
            Assert.Equal(Path.Combine(destRoot, "vids", "2025", "movie.mp4"), destination);
        });
    }

    [Fact]
    public void TryGetDestination_ReturnsFalse_WhenNoCategoryMatches()
    {
        var options = new FilesOrganizerOptions { DestinationRoot = "dest", DestinationPattern = "{Year}" };
        options.MediaCategories.Add(new MediaCategory { CategoryName = "Photos", CategoryRoot = "pics", FileExtensions = [".jpg"] });

        var mapper = new MediaFileMapper(options);
        var file = Path.Combine(Path.GetTempPath(), "doc.txt");

        var result = mapper.TryGetDestination(file, out _);
        Assert.False(result);
    }

    private static string CreateTempFile(string root, string name)
    {
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, name);
        File.WriteAllText(path, "content");
        return path;
    }

    private static void RunInTempDirectory(Action<string> test)
    {
        var dir = GetTempDirectory();
        Directory.CreateDirectory(dir);
        try
        {
            test(dir);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    private static string GetTempDirectory() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
}
