using System;
using System.Windows;

namespace MediaOrganizer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private void OnStateChanged()
        //{
        //    this.txtDestination.IsEnabled = !this.IsRunning;
        //    this.txtSource.IsEnabled = !this.IsRunning;
        //    this.btnStart.Content = this.IsRunning ? "Stop" : "Start";
        //    this.prgProgress.Visibility = this.IsRunning ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        //}

        public MainWindow()
        {
            InitializeComponent();

        }
    }
}
