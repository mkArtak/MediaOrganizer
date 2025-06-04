using MediaOrganizer.Core;
using System.Collections.Generic;
using System.IO;

namespace MediaOrganizer.Storage.Local;

internal class MediaFileMapper : IMapper
{
    private readonly List<IMapper> _mappers = new();

    public MediaFileMapper(FilesOrganizerOptions options)
    {
        foreach (var category in options.MediaCategories)
        {
            var categoryMapper = new GenericFileMapper(category.FileExtensions, Path.Combine(options.DestinationRoot, category.CategoryRoot), options.DestinationPattern);
            _mappers.Add(categoryMapper);
        }
    }

    public bool TryGetDestination(string source, out string destination)
    {
        foreach (var mapper in _mappers)
        {
            if (mapper.TryGetDestination(source, out destination))
            {
                return true;
            }
        }

        destination = null;
        return false;
    }
}
