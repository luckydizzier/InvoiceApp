using System;
using InvoiceApp.Views;

namespace InvoiceApp.Models
{
    /// <summary>
    /// Logical screens of the application for navigation purposes.
    /// </summary>
    public enum AppState
    {
        /// <summary>
        /// The application's root window.
        /// </summary>
        MainWindow,
        /// <summary>
        /// Overview dashboard showing recent activity.
        /// </summary>
        Dashboard,
        /// <summary>
        /// Lists existing invoices.
        /// </summary>
        InvoiceList,
        /// <summary>
        /// Container for creating or editing invoices.
        /// </summary>
        InvoiceEditor,
        /// <summary>
        /// Invoice header information step.
        /// </summary>
        Header,
        /// <summary>
        /// Invoice item details step.
        /// </summary>
        ItemList,
        /// <summary>
        /// Final invoice summary step.
        /// </summary>
        Summary,
        /// <summary>
        /// View for managing products.
        /// </summary>
        Products,
        /// <summary>
        /// View for managing product groups.
        /// </summary>
        ProductGroups,
        /// <summary>
        /// View for managing suppliers.
        /// </summary>
        Suppliers,
        /// <summary>
        /// View for managing tax rates.
        /// </summary>
        TaxRates,
        /// <summary>
        /// View for managing units of measure.
        /// </summary>
        Units,
        /// <summary>
        /// View for managing payment methods.
        /// </summary>
        PaymentMethods,
        /// <summary>
        /// Confirmation dialog for user actions.
        /// </summary>
        ConfirmDialog
    }

    /// <summary>
    /// Helper extensions for working with <see cref="AppState"/> values.
    /// </summary>
    public static class AppStateExtensions
    {
        /// <summary>
        /// Gets the view type associated with the specified <see cref="AppState"/>.
        /// </summary>
        /// <param name="state">The application state.</param>
        /// <returns>The view type mapped to the state.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="state"/> is not a valid enumeration value.
        /// </exception>
        public static Type GetViewType(this AppState state) => state switch
        {
            AppState.MainWindow => typeof(MainWindow),
            AppState.Dashboard => typeof(DashboardView),
            AppState.InvoiceList => typeof(InvoiceListView),
            AppState.InvoiceEditor => typeof(InvoiceEditorView),
            AppState.Header => typeof(InvoiceHeaderView),
            AppState.ItemList => typeof(InvoiceItemDataGrid),
            AppState.Summary => typeof(InvoiceSummaryPanel),
            AppState.Products => typeof(ProductView),
            AppState.ProductGroups => typeof(ProductGroupView),
            AppState.Suppliers => typeof(SupplierView),
            AppState.TaxRates => typeof(TaxRateView),
            AppState.Units => typeof(UnitView),
            AppState.PaymentMethods => typeof(PaymentMethodView),
            AppState.ConfirmDialog => typeof(ConfirmDialog),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}
