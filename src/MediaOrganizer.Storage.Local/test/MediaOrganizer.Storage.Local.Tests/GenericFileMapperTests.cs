namespace MediaOrganizer.Storage.Local.Tests;

public class GenericFileMapperTests
{
    #region Constructor tests
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPatternsIsNull()
    {
        // Arrange
        string[] patterns = null;
        string destinationRoot = "C:\\Destination";
        string destinationPattern = "{Year}\\{MonthName}";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GenericFileMapper(patterns, destinationRoot, destinationPattern));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDestinationRootIsNull()
    {
        // Arrange
        string[] patterns = new[] { ".jpg", ".png" };
        string destinationRoot = null;
        string destinationPattern = "{Year}\\{MonthName}";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GenericFileMapper(patterns, destinationRoot, destinationPattern));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDestinationPatternIsNull()
    {
        // Arrange
        string[] patterns = new[] { ".jpg", ".png" };
        string destinationRoot = "C:\\Destination";
        string destinationPattern = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GenericFileMapper(patterns, destinationRoot, destinationPattern));
    }
    #endregion

    #region TryGetDestination tests
    [Fact]
    public void TryGetDestination_ShouldReturnFalse_WhenFileIsNotMappable()
    {
        // Arrange
        var mapper = new GenericFileMapper(new[] { ".jpg", ".png" }, "C:\\Destination", "{Year}\\{MonthName}");
        string path = "C:\\Source\\file.txt";

        // Act
        bool result = mapper.TryGetDestination(path, out _);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryGetDestination_E2E()
    {
        RunInTempDirectory(root =>
        {
            // Arrange
            var sourceRoot = Path.Combine(root, "source");
            var destinationRoot = Path.Combine(root, "destination");

            var path = CreateTempFile(sourceRoot, "img.jpg");

            var mapper = new GenericFileMapper([".jpg", ".png"], destinationRoot, "{Year}\\{MonthName}");
            string expectedDirectory = Path.Combine(destinationRoot, "2023", "April");

            File.SetCreationTime(path, new DateTime(2023, 4, 1));

            // Act
            var result = mapper.TryGetDestination(path, out var destinationFilePath);

            // Assert
            Assert.True(result);

            var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            Assert.Equal(expectedDirectory, destinationDirectory);
            Assert.True(Directory.Exists(destinationDirectory));
            Assert.Equal(Path.GetFileName(path), Path.GetFileName(destinationFilePath));
        });
    }

    private string CreateTempFile(string root, string filename)
    {
        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }

        var tempFile = Path.Combine(root, filename);
        File.WriteAllText(tempFile, "dummy content");
        return tempFile;
    }

    private static void RunInTempDirectory(Action<string> test)
    {
        var tempDirectory = GetTempDirectory();
        Directory.CreateDirectory(tempDirectory);
        try
        {
            test(tempDirectory);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    private static string GetTempDirectory() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    #endregion

    [Fact]
    public void GetRelativeDestinationFolder_ShouldReturnPathWithTheNameOfTheMonth()
    {
        // Arrange
        var mapper = new GenericFileMapper(new[] { ".jpg", ".png" }, "C:\\Destination", "{Year}\\{MonthName}\\{Day}");
        var dateTaken = new DateTime(2023, 4, 10);

        // Act
        var result = mapper.GetRelativeDestinationFolder(dateTaken);

        // Assert
        Assert.Equal("2023\\April\\10", result);
    }

    [Fact]
    public void GetRelativeDestinationFolder_ShouldReturnDoubleDigitDate()
    {
        // Arrange
        var mapper = new GenericFileMapper(new[] { ".jpg", ".png" }, "C:\\Destination", "{Year}\\{MonthName}\\{Day}");
        var dateTaken = new DateTime(2023, 1, 3);

        // Act
        var result = mapper.GetRelativeDestinationFolder(dateTaken);

        // Assert
        Assert.Equal("2023\\January\\03", result);
    }

    [Fact]
    public void IsFileMappable_ShouldReturnTrue_ForSupportedExtension()
    {
        // Arrange
        var mapper = new GenericFileMapper(new[] { ".jpg", ".png" }, "C:\\Destination", "{Year}\\{MonthName}");

        // Act
        var result = mapper.IsFileMappable("image.jpg");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsFileMappable_ShouldReturnFalse_ForUnsupportedExtension()
    {
        // Arrange
        var mapper = new GenericFileMapper(new[] { ".jpg", ".png" }, "C:\\Destination", "{Year}\\{MonthName}");

        // Act
        var result = mapper.IsFileMappable("document.pdf");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsFileMappable_ShouldReturnFalse_ForEmptyFilename()
    {
        // Arrange
        var mapper = new GenericFileMapper(new[] { ".jpg", ".png" }, "C:\\Destination", "{Year}\\{MonthName}");

        // Act
        var result = mapper.IsFileMappable("");

        // Assert
        Assert.False(result);
    }
}
