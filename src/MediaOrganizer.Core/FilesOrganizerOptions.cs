namespace MediaOrganizer.Core
{
    public class FilesOrganizerOptions
    {
        public string[] VideoFileFormatPatterns { get; } = new[] { ".mov", ".mp4", ".avi", ".3gp", ".mpg" };

        public string[] ImageFileFormatPatterns { get; } = new[] { ".jpg", ".jpeg" };

        public string VideoSubfolderName { get; } = "Movies";

        public string PhotosSubfolderName { get; } = "Photos";

        public string SourceRoot { get; set; }

        public string DestinationRoot { get; set; }

        public bool RemoveSource { get; set; }
    }
}
