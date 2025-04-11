using System.Collections.Generic;
using System.Threading;

namespace MediaOrganizer.Core
{
    public interface IFileEnumerator
    {
        IEnumerable<string> GetFiles(string root);
    }
}
