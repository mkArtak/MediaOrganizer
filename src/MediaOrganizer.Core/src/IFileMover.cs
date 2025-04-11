using System.Threading.Tasks;

namespace MediaOrganizer.Core
{
    public interface IFileMover
    {
        Task MoveAsync(FileMoverOptions options, string from, string to);
    }
}
