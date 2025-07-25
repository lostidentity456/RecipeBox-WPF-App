using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RecipeBox.App.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is NOT null, return Visible.
            // If the value IS null, return Collapsed.
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is only used one-way, so we don't need to implement ConvertBack.
            throw new NotImplementedException();
        }
    }
}