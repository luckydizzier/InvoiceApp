using System;
using InvoiceApp.Views;

namespace InvoiceApp.Shared
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

        /// <summary>
        /// Gets a human readable description for the specified
        /// <see cref="AppState"/> in Hungarian.
        /// </summary>
        /// <param name="state">The application state.</param>
        /// <returns>The descriptive label.</returns>
        public static string GetDescription(this AppState state) => state switch
        {
            AppState.MainWindow => "F\u0151ablak",
            AppState.Dashboard => "Vez\u00e9rl\u0151pult",
            AppState.Header => "Fejl\u00e9c",
            AppState.ItemList => "T\u00e9telek",
            AppState.Summary => "\u00d6sszes\u00edt\u0151",
            AppState.Products => "Term\u00e9kek",
            AppState.ProductGroups => "Term\u00e9kcsoportok",
            AppState.Suppliers => "Sz\u00e1ll\u00edt\u00f3k",
            AppState.TaxRates => "\u00c1fakulcsok",
            AppState.Units => "Egys\u00e9gek",
            AppState.PaymentMethods => "Fizet\u00e9si m\u00f3dok",
            AppState.ConfirmDialog => "Meger\u0151s\u00edt\u00e9s",
            _ => state.ToString()
        };
    }
}
