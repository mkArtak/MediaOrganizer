using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaOrganizer.Core;
using MediaOrganizer.Storage.Local;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
