namespace MediaOrganizer.Core;

public record struct ProgressInfo(string FileName, int TotalFiles, int CurrentFileIndex);