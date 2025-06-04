namespace MediaOrganizer.Core;

public class MediaCategory
{
    public string CategoryName { get; set; }

    /// <summary>
    /// The directory, under which to organize all media for this category.
    /// </summary>
    public string CategoryRoot { get; set; }

    public string[] FileExtensions { get; set; }
}
