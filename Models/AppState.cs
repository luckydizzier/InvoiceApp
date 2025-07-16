using System;
using InvoiceApp.Views;

namespace InvoiceApp.Models
{
    /// <summary>
    /// Logical screens of the application for navigation purposes.
    /// </summary>
    public enum AppState
    {
        MainWindow,
        InvoiceHeader,
        InvoiceItems,
        InvoiceSummary,
        Product,
        ProductGroup,
        Supplier,
        TaxRate,
        Unit,
        PaymentMethod,
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
            AppState.InvoiceHeader => typeof(InvoiceHeaderView),
            AppState.InvoiceItems => typeof(InvoiceItemDataGrid),
            AppState.InvoiceSummary => typeof(InvoiceSummaryPanel),
            AppState.Product => typeof(ProductView),
            AppState.ProductGroup => typeof(ProductGroupView),
            AppState.Supplier => typeof(SupplierView),
            AppState.TaxRate => typeof(TaxRateView),
            AppState.Unit => typeof(UnitView),
            AppState.PaymentMethod => typeof(PaymentMethodView),
            AppState.ConfirmDialog => typeof(ConfirmDialog),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}
