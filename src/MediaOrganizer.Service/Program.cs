using MediaOrganizer.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaOrganizer.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(lb => lb.AddConsole())
            .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.Configure<FilesOrganizerOptions>(configuration.GetSection(nameof(FilesOrganizerOptions)));
                    services.AddSingleton<FilesOrganizerOptions>(sp => sp.GetRequiredService<IOptions<FilesOrganizerOptions>>().Value);
                    services.AddLocalStorageServices();

                    services.AddHostedService<Worker>();
                });
    }
}
