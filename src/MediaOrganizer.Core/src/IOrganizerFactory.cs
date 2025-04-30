namespace MediaOrganizer.Core;

public interface IOrganizerFactory
{
    IFilesOrganizer Create(FilesOrganizerOptions options);
}
