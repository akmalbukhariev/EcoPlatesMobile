using System;
using System.Globalization;
using Microsoft.Maui.Controls;
 
namespace EcoPlatesMobile.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            return value switch
            {
                int i => i != 0,
                long l => l != 0,
                double d => d != 0,
                decimal m => m != 0,
                string s => !string.IsNullOrWhiteSpace(s),
                _ => true
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
