using System;

namespace InvoiceApp.Domain
{
    public class Product : Base
    {
        public string Name { get; set; } = string.Empty;
        public decimal Net { get; set; }
        public decimal Gross { get; set; }

        public int UnitId { get; set; }
        public virtual Unit? Unit { get; set; }

        public int ProductGroupId { get; set; }
        public virtual ProductGroup? ProductGroup { get; set; }

        public int TaxRateId { get; set; }
        public virtual TaxRate? TaxRate { get; set; }

        /// <summary>
        /// Indicates whether the product is referenced by any invoice items
        /// and therefore cannot be modified or deleted.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsLocked { get; set; }
    }
}
