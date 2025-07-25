using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RecipeBox.App.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is a boolean and it's TRUE, return Visible.
            // Otherwise, return Collapsed.
            return value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}