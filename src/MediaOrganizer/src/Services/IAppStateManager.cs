using System.Threading.Tasks;

namespace MediaOrganizer.Services;

public interface IAppStateManager
{
    static char KeyValueSeparator = ':';

    static char ArrayElementsSeparator = ';';

    Task BeginLoadState();

    Task SaveStateAsync();

    Task<T> GetState<T>(string key);

    void UpdateState<T>(string key, T state);
}