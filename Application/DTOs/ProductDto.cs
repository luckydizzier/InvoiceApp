using System;

namespace InvoiceApp.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Net { get; set; }
        public decimal Gross { get; set; }
        public int UnitId { get; set; }
        public int ProductGroupId { get; set; }
        public int TaxRateId { get; set; }

        /// <summary>
        /// True when the product is referenced by invoices.
        /// </summary>
        public bool IsLocked { get; set; }
    }
}
