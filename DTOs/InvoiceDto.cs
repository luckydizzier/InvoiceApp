using System;
using System.Collections.Generic;
namespace InvoiceApp.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int SupplierId { get; set; }
        public int PaymentMethodId { get; set; }
        public bool IsGross { get; set; } = true;
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
