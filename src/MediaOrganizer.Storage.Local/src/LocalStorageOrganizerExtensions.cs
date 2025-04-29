using MediaOrganizer.Core;
using Microsoft.Extensions.DependencyInjection;

namespace MediaOrganizer.Storage.Local;

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
