using System;
using System.Collections.Generic;

namespace InvoiceApp.Domain
{
    public class InvoiceItem : Base
    {
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Indicates whether this item was aggregated from multiple entries.
        /// Not persisted in the database.
        /// </summary>
        public bool IsAggregated { get; set; }

        public int InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int TaxRateId { get; set; }
        public virtual TaxRate? TaxRate { get; set; }
    }
}
