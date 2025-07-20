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
        public DataTemplate? InvoiceTemplate { get; set; }
        public DataTemplate? PaymentMethodsTemplate { get; set; }
        public DataTemplate? SuppliersTemplate { get; set; }
        public DataTemplate? ProductGroupsTemplate { get; set; }
        public DataTemplate? TaxRatesTemplate { get; set; }
        public DataTemplate? UnitsTemplate { get; set; }
        public DataTemplate? ProductsTemplate { get; set; }
        public DataTemplate? DashboardTemplate { get; set; }
        public DataTemplate? ConfirmDialogTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not AppState state)
                return base.SelectTemplate(item, container);

            return state switch
            {
                AppState.PaymentMethods => PaymentMethodsTemplate,
                AppState.Suppliers => SuppliersTemplate,
                AppState.ProductGroups => ProductGroupsTemplate,
                AppState.TaxRates => TaxRatesTemplate,
                AppState.Units => UnitsTemplate,
                AppState.Products => ProductsTemplate,
                AppState.Dashboard => DashboardTemplate,
                AppState.Invoices => InvoiceTemplate,
                AppState.Header => InvoiceTemplate,
                AppState.ItemList => InvoiceTemplate,
                AppState.Summary => InvoiceTemplate,
                AppState.ConfirmDialog => ConfirmDialogTemplate,
                _ => InvoiceTemplate
            };
        }
    }
}
