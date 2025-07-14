using System;

namespace InvoiceApp.Models
{
    public class Invoice : Base
    {
        public string Number { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
