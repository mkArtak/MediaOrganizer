using System.Diagnostics;
using System.Windows;

namespace MediaOrganizer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

    }

    private void ReportProblem_Click(object sender, RoutedEventArgs e)
    {
        var reportProblemUrl = "https://github.com/mkArtak/MediaOrganizer/issues/new?template=bug_report.md";
        using var browserProcess = Process.Start(new ProcessStartInfo
        {
            FileName = reportProblemUrl,
            UseShellExecute = true,
        });
    }
}
