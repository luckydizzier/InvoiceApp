using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace InvoiceApp.Shared.Helpers
{
    public class BoolToRowDetailsVisibilityConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b
                ? DataGridRowDetailsVisibilityMode.Visible
                : DataGridRowDetailsVisibilityMode.Collapsed;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DataGridRowDetailsVisibilityMode mode && mode == DataGridRowDetailsVisibilityMode.Visible;
        }
    }
}
