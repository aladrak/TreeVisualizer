using System.Globalization;

namespace TreeVisualizer.App.Converters;

public class IntToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString() ?? "0";
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int.TryParse(value as string, out var res);
        return res;
    }
}