using System;

namespace InvoiceApp.Domain
{
    public class PaymentMethod : Base
    {
        public string Name { get; set; } = string.Empty;
        public int DueInDays { get; set; }
    }
}
