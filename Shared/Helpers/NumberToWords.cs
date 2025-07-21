namespace InvoiceApp.Shared.Helpers
{
    /// <summary>
    /// Provides a method to convert numeric amounts into English words.
    /// </summary>
    public static class NumberToWords
    {
        /// <summary>
        /// Converts a number into its word representation.
        /// </summary>
        public static string Convert(long number)
        {
            if (number == 0) return "zero";
            if (number < 0) return "minus " + Convert(Math.Abs(number));

            string words = string.Empty;

            if ((number / 1000) > 0)
            {
                words += Convert(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += Convert(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                {
                    words += unitsMap[number];
                }
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }
    }
}
