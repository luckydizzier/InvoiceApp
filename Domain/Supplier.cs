using System;

namespace InvoiceApp.Domain
{
    public class Supplier : Base
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? TaxId { get; set; }
        public string? BankAccntNr { get; set; }
    }
}
