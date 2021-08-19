using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StorageExtensions
    {
        public static IServiceCollection AddLocalStorageServices(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, Mapper>();
            services.AddSingleton<IFileEnumerator, PhysicalFileEnumerator>();
            services.AddSingleton<IFileMover, PhysicalFileMover>();
            services.AddSingleton<IFilesOrganizer, PhysicalFileOrganizer>();

            return services;
        }
    }
}
