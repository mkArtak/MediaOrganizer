using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaOrganizer.Storage.Local.Tests;

public class PhysicalFileOrganizerTests
{
    [Fact]
    public async Task OrganizeAsync_MovesFilesSuccessfully()
    {
        // Arrange
        var files = new List<string> { "file1.txt", "file2.txt" };

        var fileMoverMock = new Mock<IFileMover>();
        var loggerMock = new Mock<ILogger>();
        var createDirectoryIfNoExistHandlerMock = new Mock<Action<string>>();
        var options = new FilesOrganizerOptions
        {
            SourceRoot = "C:\\Source",
            DestinationRoot = "C:\\Destination",
            RemoveSource = false,
            SkipExistingFiles = true
        };

        var fileEnumeratorMock = new Mock<IFileEnumerator>();
        fileEnumeratorMock.Setup(e => e.GetFiles(options.SourceRoot)).Returns(files);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.TryGetDestination(It.IsAny<string>(), out It.Ref<string>.IsAny))
                  .Returns((string source, out string destination) =>
                  {
                      destination = Path.Combine(options.DestinationRoot, Path.GetFileName(source));
                      return true;
                  });

        var organizer = new PhysicalFileOrganizer(options, fileMoverMock.Object, fileEnumeratorMock.Object, mapperMock.Object, createDirectoryIfNoExistHandlerMock.Object, loggerMock.Object);
        var progressMock = new Mock<IProgress<ProgressInfo>>();
        var cancellationToken = CancellationToken.None;

        // Act
        await organizer.OrganizeAsync(progressMock.Object, cancellationToken);

        // Assert
        fileMoverMock.Verify(m => m.MoveAsync(It.IsAny<FileMoverOptions>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(files.Count));
    }
}
