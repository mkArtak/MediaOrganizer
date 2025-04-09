namespace MediaOrganizer.Core;

public interface IMapper
{
    bool TryGetDestination(string source, out string destination);
}
