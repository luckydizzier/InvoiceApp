using System;

namespace InvoiceApp.Models
{
    public class SampleDataOptions
    {
        public int SupplierCount { get; set; } = 1;
        public int ProductGroupCount { get; set; } = 1;
        public int ProductCount { get; set; } = 1;
        public int InvoiceCount { get; set; } = 1;
        public int ItemsPerInvoice { get; set; } = 1;

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
