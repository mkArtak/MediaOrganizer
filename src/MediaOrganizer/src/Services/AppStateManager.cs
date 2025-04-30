using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace MediaOrganizer.Services;

internal class AppStateManager : IAppStateManager
{
    private readonly ConcurrentDictionary<string, string> _state = new();

    private string _filename = "App.data";
    private Task _loadTask;

    public AppStateManager()
    {

    }

    public Task BeginLoadState()
    {
        _loadTask = LoadStateAsync();
        return _loadTask;
    }

    public async Task SaveStateAsync()
    {
        IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();

        using IsolatedStorageFileStream stream = storage.OpenFile(_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using StreamWriter writer = new StreamWriter(stream);

        foreach (var state in _state)
        {
            var lineToStore = state.Key + IAppStateManager.ExtensionsSeparator + state.Value;
            await writer.WriteLineAsync(lineToStore);
        }
    }

    private async Task LoadStateAsync()
    {
        _state.Clear();
        IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
        try
        {
            if (storage.FileExists(_filename))
            {
                using IsolatedStorageFileStream stream = storage.OpenFile(_filename, FileMode.Open, FileAccess.Read);
                using StreamReader reader = new StreamReader(stream);
                // Restore each application-scope property individually
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    string[] keyValue = line.Split(IAppStateManager.ExtensionsSeparator);
                    if (keyValue.Length != 2)
                    {
                        // Skip loading this particular setting as the data is corrupt
                        continue;
                    }

                    _state.TryAdd(keyValue[0], keyValue[1]);
                }
            }
        }
        catch (DirectoryNotFoundException ex)
        {
            // Path the file didn't exist
        }
        catch (IsolatedStorageException ex)
        {
            // Storage was removed or doesn't exist
            // -or-
            // If using .NET 6+ the inner exception contains the real cause
        }
    }

    public async Task<T> GetState<T>(string key)
    {
        await (_loadTask ?? LoadStateAsync());

        if (_state.TryGetValue(key, out string value))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        return default;
    }

    public void UpdateState<T>(string key, T state)
    {
        if (_state.ContainsKey(key))
        {
            _state[key] = state.ToString();
        }
        else
        {
            _state.TryAdd(key, state.ToString());
        }
    }
}
