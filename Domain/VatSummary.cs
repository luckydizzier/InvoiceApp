namespace InvoiceApp.Domain
{
    /// <summary>
    /// Represents VAT breakdown for a specific tax rate.
    /// </summary>
    public class VatSummary
    {
        public decimal Rate { get; init; }
        public decimal Net { get; init; }
        public decimal Vat { get; init; }
        public decimal Gross => Net + Vat;
    }
}
