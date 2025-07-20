using System;

namespace InvoiceApp.Domain
{
    public class SampleDataOptions
    {
        public int SupplierCount { get; set; } = 1;
        public int ProductGroupCount { get; set; } = 1;
        public int ProductCount { get; set; } = 1;
        public int InvoiceCount { get; set; } = 1;

        /// <summary>
        /// Minimum number of items generated for each invoice.
        /// </summary>
        public int ItemsPerInvoiceMin { get; set; } = 1;

        /// <summary>
        /// Maximum number of items generated for each invoice.
        /// </summary>
        public int ItemsPerInvoiceMax { get; set; } = 1;

        /// <summary>
        /// Minimum quantity for generated invoice items.
        /// </summary>
        public decimal ItemQuantityMin { get; set; } = 1m;

        /// <summary>
        /// Maximum quantity for generated invoice items.
        /// </summary>
        public decimal ItemQuantityMax { get; set; } = 5m;
    }
}
