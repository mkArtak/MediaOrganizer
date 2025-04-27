using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaOrganizer.Converters;

public class BooleanToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool running)
            return running ? "Stop" : "Start organizing";

        return "Start organizing";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
