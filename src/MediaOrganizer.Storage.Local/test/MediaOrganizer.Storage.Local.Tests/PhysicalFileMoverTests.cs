using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaOrganizer.Storage.Local.Tests;

public class PhysicalFileMoverTests
{
    [Fact]
    public async Task MoveAsync_CallsMoveHandlerForTheSpecifiedFile()
    {
        // Arrange
        var moveHandler = new Mock<Action<string, string>>();
        var copyHandler = new Mock<Action<string, string>>();

        var logger = new Mock<ILogger>();

        var fileMover = new PhysicalFileMover(copyHandler.Object, moveHandler.Object, logger.Object);

        var options = new FileMoverOptions
        {
            RemoveSourceAfterMove = true,
            SkipIfFileExists = false
        };

        var sourceFile = "source.txt";
        var destinationFile = "destination.txt";
        moveHandler.Setup(m => m(sourceFile, destinationFile)).Verifiable();


        // Act
        await fileMover.MoveAsync(options, sourceFile, destinationFile);

        // Assert
        moveHandler.Verify(m => m(sourceFile, destinationFile), Times.Once);
        copyHandler.Verify(m => m(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
