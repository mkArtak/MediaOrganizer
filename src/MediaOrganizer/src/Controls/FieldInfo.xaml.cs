using System.Windows;
using System.Windows.Controls;

namespace MediaOrganizer.Controls;

/// <summary>
/// Interaction logic for FieldInfo.xaml
/// </summary>
public partial class FieldInfo : UserControl
{
    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(nameof(InformationText), typeof(string), typeof(FieldInfo));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public FieldInfo()
    {
        InitializeComponent();
    }
}
