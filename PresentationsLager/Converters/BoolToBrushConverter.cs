using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PresentationsLager.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public static readonly BoolToBrushConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return new SolidColorBrush(Color.FromRgb(244, 185, 66)); // #F4B942 - RestoNation accent color
            }
            return new SolidColorBrush(Color.FromRgb(220, 220, 220)); // Light gray for unselected
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}