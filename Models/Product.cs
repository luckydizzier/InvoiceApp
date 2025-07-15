using System;

namespace InvoiceApp.Models
{
    public class Product : Base
    {
        public string Name { get; set; } = string.Empty;
        public decimal Net { get; set; }
        public decimal Gross { get; set; }

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }

        public int ProductGroupId { get; set; }
        public ProductGroup? ProductGroup { get; set; }

        public int TaxRateId { get; set; }
        public TaxRate? TaxRate { get; set; }
    }
}
