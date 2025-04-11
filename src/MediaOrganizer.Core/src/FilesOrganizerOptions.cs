using System;

namespace MediaOrganizer.Core
{
    public class FilesOrganizerOptions
    {
        public static readonly string DefaultVideoSubfolderName = "Movies";

        public static readonly string DefaultPhotosSubfolderName = "Photos";

        public static readonly string DefaultDestinationPattern = "{Year}/{MonthName}/{Year}-{Month}-{Day}";

        public static readonly string[] DefaultVideoFileFormatPatterns = { ".mov", ".mp4", ".avi", ".3gp", ".mpg" };

        public static readonly string[] DefaultPhotoFileFormatPatterns = { ".jpg", ".jpeg", ".png" };

        public string[] VideoFileFormatPatterns { get; set; } = DefaultVideoFileFormatPatterns;

        public string[] ImageFileFormatPatterns { get; set; } = DefaultPhotoFileFormatPatterns;

        public string VideoSubfolderName { get; set; } = DefaultVideoSubfolderName;

        public string PhotosSubfolderName { get; set; } = DefaultPhotosSubfolderName;

        public string SourceRoot { get; set; }

        public string DestinationRoot { get; set; }

        public bool RemoveSource { get; set; }

        public bool SkipExistingFiles { get; set; }

        public string DestinationPattern { get; set; } = DefaultDestinationPattern;

        public override string ToString()
        {
            return $"VideoSubfolderName: {VideoSubfolderName}{Environment.NewLine}PhotoSubfolderName: {PhotosSubfolderName}{Environment.NewLine}SourceRoot: {SourceRoot}{Environment.NewLine}DestinationRoot: {DestinationRoot}{Environment.NewLine}RemoveSource: {RemoveSource}{Environment.NewLine}SkipExistingFiles: {SkipExistingFiles}";
        }
    }
}
