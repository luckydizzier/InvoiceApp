using System.Windows;
using InvoiceApp.Domain;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class SampleDataDialog : Window
    {
        public SampleDataOptions Options { get; }

        public SampleDataDialog()
        {
            InitializeComponent();
            Log.Information("SampleDataDialog initialized");
            Options = new SampleDataOptions
            {
                SupplierCount = 3,
                ProductGroupCount = 3,
                ProductCount = 5,
                InvoiceCount = 10,
                ItemsPerInvoiceMin = 1,
                ItemsPerInvoiceMax = 2,
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
