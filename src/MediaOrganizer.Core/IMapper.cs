namespace MediaOrganizer.Core
{
    public interface IMapper
    {
        string GetDestination(FilesOrganizerOptions options, string source);
    }
}
