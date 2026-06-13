using MediaOrganizer.Storage.Local;

namespace MediaOrganizer.Storage.Local.Tests;

public class ExifCreationDateExtractorTests
{
    [Fact]
    public void TryParseMetadataTimestamp_ShouldPreserveExifDateWithoutAssumingUtc()
    {
        var result = ExifCreationDateExtractor.TryParseMetadataTimestamp("2025:04:01 00:30:00", out var captureDate);

        Assert.True(result);
        Assert.Equal(new DateTime(2025, 4, 1, 0, 30, 0), captureDate);
        Assert.Equal(DateTimeKind.Unspecified, captureDate.Kind);
    }

    [Fact]
    public void TryParseMetadataTimestamp_ShouldPreserveOffsetAwareCalendarDate()
    {
        var result = ExifCreationDateExtractor.TryParseMetadataTimestamp("Tue Apr 01 00:30:00 +09:00 2025", out var captureDate);

        Assert.True(result);
        Assert.Equal(new DateTime(2025, 4, 1, 0, 30, 0), captureDate);
    }

    [Fact]
    public void TryParseMetadataTimestamp_ShouldSupportGeneralOffsetParsingWithoutUtcShift()
    {
        var result = ExifCreationDateExtractor.TryParseMetadataTimestamp("2025-04-01T00:30:00+09:00", out var captureDate);

        Assert.True(result);
        Assert.Equal(new DateTime(2025, 4, 1, 0, 30, 0), captureDate);
    }

    [Fact]
    public void TryParseMetadataTimestamp_ShouldReturnFalseForUnknownFormat()
    {
        var result = ExifCreationDateExtractor.TryParseMetadataTimestamp("not-a-date", out _);

        Assert.False(result);
    }
}
