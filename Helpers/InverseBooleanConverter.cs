using System;
using System.Globalization;
using System.Windows.Data;

namespace InvoiceApp.Helpers
{
    /// <summary>
    /// Simple converter that negates a boolean value.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class InverseBooleanConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;
    }
}
