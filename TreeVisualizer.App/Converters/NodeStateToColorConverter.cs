using System.Globalization;
using TreeVisualizer.Domain.Enums;

namespace TreeVisualizer.App.Converters;

public sealed class NodeStateToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is NodeVisualState state
            ? state switch
            {
                NodeVisualState.Current => Colors.DeepSkyBlue,
                NodeVisualState.Compared => Colors.Gold,
                NodeVisualState.Found => Colors.LightGreen,
                NodeVisualState.Inserted => Colors.MediumSeaGreen,
                NodeVisualState.Deleted => Colors.OrangeRed,
                NodeVisualState.Rotated => Colors.MediumPurple,
                NodeVisualState.Split => Colors.SandyBrown,
                NodeVisualState.Error => Colors.IndianRed,
                _ => Colors.White
            }
            : Colors.White;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
