using System;

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
}
