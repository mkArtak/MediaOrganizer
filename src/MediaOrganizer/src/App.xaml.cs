using MediaOrganizer.Services;
using System.Windows;

namespace MediaOrganizer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        AppStateManager.Instance.BeginLoadState();
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        AppStateManager.Instance.SaveStateAsync().GetAwaiter().GetResult();
    }
}
