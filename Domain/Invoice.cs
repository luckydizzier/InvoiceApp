using System;
using System.Collections.Generic;

namespace InvoiceApp.Domain
{
    public class Invoice : Base
    {
        public string Number { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        public int PaymentMethodId { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }

        public bool IsGross { get; set; } = true;

        public virtual List<InvoiceItem> Items { get; set; } = new();
    }
}
