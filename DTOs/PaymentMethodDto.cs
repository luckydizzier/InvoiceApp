using System;

namespace InvoiceApp.DTOs
{
    public class PaymentMethodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DueInDays { get; set; }
    }
}
