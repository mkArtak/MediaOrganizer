using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Core
{
    public interface IFilesOrganizer
    {
        Task OrganizeAsync(FilesOrganizerOptions options, CancellationToken token);
    }
}
