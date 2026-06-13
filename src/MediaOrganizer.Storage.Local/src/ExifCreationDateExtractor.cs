using MetadataExtractor;
using System;
using System.IO;
using System.Globalization;

namespace MediaOrganizer.Storage.Local;

internal sealed class ExifCreationDateExtractor : FileInfoBasedCreationDateExtractor
{
    private static readonly string[] SupportedTagNames =
    [
        "Date/Time Original",
        "Date/Time Digitized",
        "Date/Time",
        "File Modified Date"
    ];

    private static readonly string[] OffsetAwareFormats =
    [
        "ddd MMM dd HH:mm:ss zzz yyyy",
        "ddd MMM d HH:mm:ss zzz yyyy"
    ];

    public ExifCreationDateExtractor()
    {
    }

    public override DateTime ExtractCreationDate(FileInfo file)
    {
        var metadata = ImageMetadataReader.ReadMetadata(file.FullName);
        foreach (var item in metadata)
        {
            foreach (var tag in item.Tags)
            {
                if (SupportedTagNames.Contains(tag.Name, StringComparer.OrdinalIgnoreCase) &&
                    TryParseMetadataTimestamp(tag.Description, out var captureDate))
                {
                    return captureDate;
                }
            }
        }

        return base.ExtractCreationDate(file);
    }

    internal static bool TryParseMetadataTimestamp(string value, out DateTime captureDate)
    {
        captureDate = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        // EXIF numeric timestamps typically do not carry timezone information,
        // so preserve the recorded calendar date instead of treating it as UTC.
        if (DateTime.TryParseExact(value, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var originalDate))
        {
            captureDate = DateTime.SpecifyKind(originalDate, DateTimeKind.Unspecified);
            return true;
        }

        if (DateTimeOffset.TryParseExact(value, OffsetAwareFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
        {
            captureDate = dto.DateTime;
            return true;
        }

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dto))
        {
            captureDate = dto.DateTime;
            return true;
        }

        return false;
    }
}
