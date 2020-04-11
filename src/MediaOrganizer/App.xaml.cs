using MediaOrganizer.ViewModels;
using MediaOrganizer.Views;
using Prism.Mvvm;
using System.Windows;

namespace MediaOrganizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //ViewModelLocationProvider.Register<MainWindow, FileOrganizerViewModel>();
        }
    }
}
