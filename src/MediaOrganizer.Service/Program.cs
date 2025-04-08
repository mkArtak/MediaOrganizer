using MediaOrganizer.Core;
using MediaOrganizer.Service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLocalStorageServices();
builder.Services.AddHostedService<Worker>();

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<FilesOrganizerOptions>(configuration.GetSection(nameof(FilesOrganizerOptions)));

var host = builder.Build();
host.Run();


/*
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureLogging(lb => lb.AddConsole())
        .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(lb => lb.AddConsole());
                IConfiguration configuration = hostContext.Configuration;
                services.Configure<FilesOrganizerOptions>(configuration.GetSection(nameof(FilesOrganizerOptions)));
                services.AddSingleton<FilesOrganizerOptions>(sp => sp.GetRequiredService<IOptions<FilesOrganizerOptions>>().Value);
                services.AddLocalStorageServices();

                services.AddHostedService<Worker>();
            });
}

*/