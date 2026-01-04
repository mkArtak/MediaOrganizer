using MetadataExtractor;
using System;
using System.IO;
using System.Globalization;

namespace MediaOrganizer.Storage.Local;

internal sealed class ExifCreationDateExtractor : FileInfoBasedCreationDateExtractor
{
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
                if (tag.Name.Equals("Date/Time Original", StringComparison.OrdinalIgnoreCase) ||
                    tag.Name.Equals("Date/Time Digitized", StringComparison.OrdinalIgnoreCase) ||
                    tag.Name.Equals("Date/Time", StringComparison.OrdinalIgnoreCase) ||
                    tag.Name.Equals("File Modified Date", StringComparison.OrdinalIgnoreCase))
                {
                    // Try EXIF numeric format first (e.g. "2025:04:08 19:37:09")
                    if (DateTime.TryParseExact(tag.Description, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var originalDate))
                    {
                        return originalDate.ToUniversalTime();
                    }

                    // Exact parse for textual offset-aware formats like: "Tue Apr 08 19:37:09 -07:00 2025"
                    var formats = new[]
                    {
                        "ddd MMM dd HH:mm:ss zzz yyyy",
                        "ddd MMM d HH:mm:ss zzz yyyy"
                    };

                    if (DateTimeOffset.TryParseExact(tag.Description, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
                    {
                        return dto.UtcDateTime;
                    }

                    // Final fallback: a general DateTimeOffset parse (less strict)
                    if (DateTimeOffset.TryParse(tag.Description, CultureInfo.InvariantCulture, DateTimeStyles.None, out dto))
                    {
                        return dto.UtcDateTime;
                    }
                }
            }
        }

        return base.ExtractCreationDate(file);
    }
}
