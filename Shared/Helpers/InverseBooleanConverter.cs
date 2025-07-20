using System;
using System.Globalization;
using System.Windows.Data;

namespace InvoiceApp.Shared.Helpers
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;
    }
}
