using System;

namespace InvoiceApp.DTOs
{
    public class TaxRateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
