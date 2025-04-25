using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MediaOrganizer.Converters;

public class FileExtensionsArrayToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string[] extensions)
        {
            return string.Join(", ", extensions.Select(ext => ext.StartsWith(".") ? ext : "." + ext));
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string extensionsString)
        {
            return extensionsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(ext => ext.Trim().StartsWith(".") ? ext.Trim() : "." + ext.Trim())
                                   .ToArray();
        }

        return null;
    }
}
