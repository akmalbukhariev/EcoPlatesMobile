using System.Globalization;

namespace EcoPlatesMobile.Converters;

public class StringToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return 0.0;

        var s = value.ToString()?.Trim();
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d)) return d;

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
