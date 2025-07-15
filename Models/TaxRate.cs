using System;

namespace InvoiceApp.Models
{
    public class TaxRate : Base
    {
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
