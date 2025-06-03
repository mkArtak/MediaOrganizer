using MediaOrganizer.Services;
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
        _serviceProvider.GetRequiredService<IAppStateManager>().BeginLoadState();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.GetRequiredService<IAppStateManager>().SaveStateAsync().GetAwaiter().GetResult();

        base.OnExit(e);
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddLogging(lb => lb.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Information));

        services.AddLocalStorageOrganizer();

        services.AddSingleton<IAppStateManager, AppStateManager>();
        services.AddSingleton<FileOrganizerViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
