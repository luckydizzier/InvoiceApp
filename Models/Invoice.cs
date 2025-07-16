using System;
using System.Collections.Generic;

namespace InvoiceApp.Models
{
    public class Invoice : Base
    {
        public string Number { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public bool IsGross { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
    }
}
