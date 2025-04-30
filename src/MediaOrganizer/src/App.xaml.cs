using MediaOrganizer.Services;
using MediaOrganizer.Storage.Local;
using MediaOrganizer.ViewModels;
using MediaOrganizer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace MediaOrganizer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var sc = new ServiceCollection();
        ConfigureServices(sc);
        _serviceProvider = sc.BuildServiceProvider();

        // This will cause the `ApplicationStartup event to file, and the Application_Startup handler to be called subsequently.
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddLogging(lb => lb.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Information));

        services.AddLocalStorageOrganizer();

        services.AddSingleton<IAppStateManager, AppStateManager>();
        services.AddSingleton<FileOrganizerViewModel>();
        services.AddSingleton<MainWindow>();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        _serviceProvider.GetRequiredService<IAppStateManager>().BeginLoadState();
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _serviceProvider.GetRequiredService<IAppStateManager>().SaveStateAsync().GetAwaiter().GetResult();
    }
}
