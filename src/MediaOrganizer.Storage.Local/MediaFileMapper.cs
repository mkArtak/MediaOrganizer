using MediaOrganizer.Core;

namespace MediaOrganizer.Storage.Local;

internal class MediaFileMapper : IMapper
{
    private readonly IMapper imageMapper;
    private readonly IMapper videoMapper;

    public MediaFileMapper(FilesOrganizerOptions options)
    {
        this.imageMapper = new GenericFileMapper(options.ImageFileFormatPatterns, options.DestinationRoot, options.DestinationPattern);
        this.videoMapper = new GenericFileMapper(options.VideoFileFormatPatterns, options.DestinationRoot, options.DestinationPattern);
    }

    public bool TryGetDestination(string source, out string destination)
    {
        if (imageMapper.TryGetDestination(source, out destination))
        {
            return true;
        }
        if (videoMapper.TryGetDestination(source, out destination))
        {
            return true;
        }

        return false;
    }
}
