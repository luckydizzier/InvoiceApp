using System;
using System.Windows;
using System.Windows.Controls;
using InvoiceApp.Models;

namespace InvoiceApp.Helpers
{
    /// <summary>
    /// Selects a DataTemplate based on <see cref="AppState"/> enumeration values.
    /// </summary>
    public class AppStateTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? MainWindowTemplate { get; set; }
        public DataTemplate? PaymentMethodTemplate { get; set; }
        public DataTemplate? SupplierTemplate { get; set; }
        public DataTemplate? ProductGroupTemplate { get; set; }
        public DataTemplate? TaxRateTemplate { get; set; }
        public DataTemplate? UnitTemplate { get; set; }
        public DataTemplate? ProductTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not AppState state)
                return base.SelectTemplate(item, container);

            return state switch
            {
                AppState.PaymentMethod => PaymentMethodTemplate,
                AppState.Supplier => SupplierTemplate,
                AppState.ProductGroup => ProductGroupTemplate,
                AppState.TaxRate => TaxRateTemplate,
                AppState.Unit => UnitTemplate,
                AppState.Product => ProductTemplate,
                _ => MainWindowTemplate
            };
        }
    }
}
