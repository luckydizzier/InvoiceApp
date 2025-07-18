namespace InvoiceApp.Helpers
{
    /// <summary>
    /// Utility class for calculating invoice amounts.
    /// </summary>
    public static class AmountCalculator
    {
        public struct Amounts
        {
            public decimal Net { get; init; }
            public decimal Vat { get; init; }
            public decimal Gross => Net + Vat;
        }

        /// <summary>
        /// Calculates net, VAT and gross amounts for an item.
        /// </summary>
        public static Amounts Calculate(decimal quantity, decimal unitPrice, decimal taxRate, bool isGross)
        {
            decimal net = isGross
                ? quantity * unitPrice / (1m + taxRate / 100m)
                : quantity * unitPrice;
            decimal vat = net * taxRate / 100m;
            return new Amounts { Net = net, Vat = vat };
        }
    }
}
