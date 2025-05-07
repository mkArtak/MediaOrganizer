using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local;

namespace Microsoft.Extensions.DependencyInjection;

public static class LocalStorageOrganizerExtensions
{
    public static IServiceCollection AddLocalStorageOrganizer(this IServiceCollection services)
    {
        services.AddSingleton<IOrganizerFactory, PhysicalFilesOrganizerFactory>();
        services.AddSingleton<IFileMover, PhysicalFileMover>();
        services.AddSingleton<IFileEnumerator, PhysicalFileEnumerator>();

        return services;
    }
}
