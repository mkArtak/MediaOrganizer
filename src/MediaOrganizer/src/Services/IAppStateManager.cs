using System.Threading.Tasks;

namespace MediaOrganizer.Services
{
    internal interface IAppStateManager
    {
        static char ExtensionsSeparator = ':';
        Task BeginLoadState();
        Task<T> GetState<T>(string key);
        void UpdateState<T>(string key, T state);
    }
}