using System.Collections.Generic;

namespace MediaOrganizer.Core
{
    public interface IFileEnumerator
    {
        IEnumerable<string> GetFilesAsync(string root);
    }
}
