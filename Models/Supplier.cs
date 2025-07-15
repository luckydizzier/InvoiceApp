using System;

namespace InvoiceApp.Models
{
    public class Supplier : Base
    {
        public string Name { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? BankAccntNr { get; set; }
    }
}
