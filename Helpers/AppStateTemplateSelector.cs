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
        public DataTemplate? PaymentMethodViewTemplate { get; set; }
        public DataTemplate? SupplierTemplate { get; set; }
        public DataTemplate? ProductGroupTemplate { get; set; }
        public DataTemplate? TaxRateTemplate { get; set; }
        public DataTemplate? UnitTemplate { get; set; }
        public DataTemplate? ProductViewTemplate { get; set; }
        public DataTemplate? DashboardTemplate { get; set; }
        public DataTemplate? InvoiceListTemplate { get; set; }
        public DataTemplate? InvoiceEditorTemplate { get; set; }
        public DataTemplate? InvoiceHeaderTemplate { get; set; }
        public DataTemplate? InvoiceItemsTemplate { get; set; }
        public DataTemplate? InvoiceSummaryTemplate { get; set; }
        public DataTemplate? ConfirmDialogTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not AppState state)
                return base.SelectTemplate(item, container);

            return state switch
            {
                AppState.PaymentMethodView => PaymentMethodViewTemplate,
                AppState.Supplier => SupplierTemplate,
                AppState.ProductGroup => ProductGroupTemplate,
                AppState.TaxRate => TaxRateTemplate,
                AppState.Unit => UnitTemplate,
                AppState.ProductView => ProductViewTemplate,
                AppState.Dashboard => DashboardTemplate,
                AppState.InvoiceList => InvoiceListTemplate,
                AppState.InvoiceEditor => InvoiceEditorTemplate,
                AppState.InvoiceHeader => InvoiceHeaderTemplate,
                AppState.InvoiceItems => InvoiceItemsTemplate,
                AppState.InvoiceSummary => InvoiceSummaryTemplate,
                AppState.ConfirmDialog => ConfirmDialogTemplate,
                _ => MainWindowTemplate
            };
        }
    }
}
