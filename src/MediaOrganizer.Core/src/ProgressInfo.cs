namespace MediaOrganizer.Core;

/// <summary>
/// Represents a reportable unit of progress in file organization process
/// </summary>
/// <param name="FileName">The name of the file for which the progress is reported</param>
/// <param name="TotalFiles">Total number of files of the overall organization process.</param>
/// <param name="CurrentFileIndex">The current index of the file out of the <see cref="TotalFiles"/>.</param>
/// <param name="Success">A boolean value indicating whether the file was process successfully or not.</param>
public record struct ProgressInfo(string FileName, int TotalFiles, int CurrentFileIndex, bool Success = true);