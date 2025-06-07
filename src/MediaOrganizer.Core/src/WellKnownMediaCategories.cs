namespace MediaOrganizer.Core;

public static class WellKnownMediaCategories
{
    public static readonly MediaCategory Movies = new MediaCategory { CategoryName = "Movies", CategoryRoot = "Movies", FileExtensions = [".mov", ".mp4", ".avi", ".3gp", ".mpg", ".mkv", ".wmv", ".flv", ".m4v", ".webm"] };

    public static readonly MediaCategory Photos = new MediaCategory { CategoryName = "Photos", CategoryRoot = "Photos", FileExtensions = [".jpg", ".jpeg", ".png", ".heic", ".bmp", ".gif", ".tiff", ".tif", ".webp"] };
}
