using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace MediaOrganizer.Services;

internal class AppStateManager : IAppStateManager
{
    private readonly ConcurrentDictionary<string, string> _state = new();
    private readonly ILogger _logger;
    private string _filename = "App.data";
    private Task _loadTask;

    public AppStateManager(ILogger<AppStateManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task BeginLoadState()
    {
        return _loadTask ??= LoadStateAsync();
    }

    public async Task SaveStateAsync()
    {
        // Wait for the previous load operation to complete first.
        if (_loadTask is not null)
            await _loadTask;

        _logger.LogInformation($"Saving application state to file: {_filename}");
        try
        {
            using IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using IsolatedStorageFileStream stream = storage.OpenFile(_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using StreamWriter writer = new StreamWriter(stream);

            foreach (var state in _state)
            {
                var lineToStore = state.Key + IAppStateManager.ExtensionsSeparator + state.Value;
                await writer.WriteLineAsync(lineToStore);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving application state. Reason: {ex.Message}");
        }
    }

    private async Task LoadStateAsync()
    {
        _logger.LogTrace($"Loading application state");
        try
        {
            using IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();

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
                        _logger.LogWarning($"Invalid line in state file: {line}. Expected format: key{IAppStateManager.ExtensionsSeparator}value");
                        continue;
                    }

                    _state.TryAdd(keyValue[0], keyValue[1]);
                }
            }
            else
            {
                _logger.LogWarning($"State file not found: {_filename}. No state loaded.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading application state. Reason: {ex.Message}");
        }
    }

    public async Task<T> GetState<T>(string key)
    {
        await _loadTask;

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
