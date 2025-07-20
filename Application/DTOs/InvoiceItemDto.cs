using System;

namespace InvoiceApp.Application.DTOs
{
    public class InvoiceItemDto
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int TaxRateId { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
