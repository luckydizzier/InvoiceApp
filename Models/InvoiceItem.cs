using System;
using System.Collections.Generic;

namespace InvoiceApp.Models
{
    public class InvoiceItem : Base
    {
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Deposit { get; set; }
        public decimal Return { get; set; }

        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int TaxRateId { get; set; }
        public TaxRate? TaxRate { get; set; }
    }
}
