using System;

namespace MediaOrganizer.Core
{
    public class FilesOrganizerOptions
    {
        public string[] VideoFileFormatPatterns { get; } = new[] { ".mov", ".mp4", ".avi", ".3gp", ".mpg" };

        public string[] ImageFileFormatPatterns { get; } = new[] { ".jpg", ".jpeg", ".png" };

        public string VideoSubfolderName { get; set; }

        public string PhotosSubfolderName { get; set; }

        public string SourceRoot { get; set; }

        public string DestinationRoot { get; set; }

        public bool RemoveSource { get; set; }

        public bool SkipExistingFiles { get; set; }

        public string DestinationPattern { get; set; }

        public override string ToString()
        {
            return $"VideoSubfolderName: {VideoSubfolderName}{Environment.NewLine}PhotoSubfolderName: {PhotosSubfolderName}{Environment.NewLine}SourceRoot: {SourceRoot}{Environment.NewLine}DestinationRoot: {DestinationRoot}{Environment.NewLine}RemoveSource: {RemoveSource}{Environment.NewLine}SkipExistingFiles: {SkipExistingFiles}";
        }
    }
}
