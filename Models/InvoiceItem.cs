using System;
using System.Collections.Generic;

namespace InvoiceApp.Models
{
    public class InvoiceItem : Base
    {
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int TaxRateId { get; set; }
        public TaxRate? TaxRate { get; set; }
    }
}
