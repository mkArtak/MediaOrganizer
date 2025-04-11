using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Core
{
    public interface IFilesOrganizer
    {
        Task OrganizeAsync(IProgress<ProgressInfo> progress, CancellationToken token);
    }
}
