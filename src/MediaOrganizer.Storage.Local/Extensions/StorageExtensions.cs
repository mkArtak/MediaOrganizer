using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class StorageExtensions
{
    public static IServiceCollection AddLocalStorageServices(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, Mapper>();
        services.AddSingleton<IFileEnumerator, PhysicalFileEnumerator>();
        services.AddTransient<IFileMover>(sp => new PhysicalFileMover(sp.GetRequiredService<ILogger<PhysicalFileMover>>()));
        services.AddTransient<IFilesOrganizer, PhysicalFileOrganizer>();

        return services;
    }
}
