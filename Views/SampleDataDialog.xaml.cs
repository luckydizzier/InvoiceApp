using System.Windows;
using InvoiceApp.Models;

namespace InvoiceApp.Views
{
    public partial class SampleDataDialog : Window
    {
        public SampleDataOptions Options { get; }

        public SampleDataDialog()
        {
            InitializeComponent();
            Options = new SampleDataOptions
            {
                SupplierCount = 3,
                ProductGroupCount = 3,
                ProductCount = 5,
                InvoiceCount = 10,
                ItemsPerInvoice = 2,
                ItemQuantityMin = 1m,
                ItemQuantityMax = 5m
            };
            DataContext = Options;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
