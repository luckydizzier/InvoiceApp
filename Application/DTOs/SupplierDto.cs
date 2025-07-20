using System;

namespace InvoiceApp.Application.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? TaxId { get; set; }
        public string? BankAccntNr { get; set; }
    }
}
