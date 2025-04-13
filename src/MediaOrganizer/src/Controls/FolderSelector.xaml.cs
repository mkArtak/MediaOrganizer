using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace MediaOrganizer.Controls;

/// <summary>
/// Interaction logic for FolderSelector.xaml
/// </summary>
public partial class FolderSelector : UserControl
{
    public static readonly DependencyProperty SelectedFolderProperty =
          DependencyProperty.Register(nameof(SelectedFolder), typeof(string), typeof(FolderSelector),
              new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedFolderChanged));

    public string SelectedFolder
    {
        get => (string)GetValue(SelectedFolderProperty);
        set => SetValue(SelectedFolderProperty, value);
    }

    public FolderSelector()
    {
        InitializeComponent();
    }

    private static void OnSelectedFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FolderSelector control)
        {
            control.OnPropertyChanged(new DependencyPropertyChangedEventArgs(SelectedFolderProperty, e.OldValue, e.NewValue));
        }
    }

    private void SelectFolder(object sender, System.Windows.RoutedEventArgs e)
    {
        if (TrySelectFolder(out var folder))
            SelectedFolder = folder;
    }

    private static bool TrySelectFolder(out string folderPath)
    {
        var dialog = new OpenFolderDialog();
        dialog.Multiselect = false;
        if (dialog.ShowDialog() != true)
        {
            folderPath = null;
            return false;
        }

        folderPath = dialog.FolderName;
        return true;
    }
}
