using System;
using System.Threading.Tasks;

namespace MediaOrganizer.Services;

internal static class AppStateExtensions
{
    public static async Task<T> GetStateOrDefault<T>(this IAppStateManager stateManager, string key, T defaultValue = default, Func<string, T> transformer = null)
    {
        if (stateManager == null)
            throw new ArgumentNullException(nameof(stateManager));

        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var state = await stateManager.GetState<string>(key);
        if (state is not null && transformer is not null)
            return transformer(state) ?? defaultValue;
        else
            return defaultValue;
    }
}
