using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class InvoiceHeaderView : UserControl
    {
        public InvoiceHeaderView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Log.Information("InvoiceHeaderView loaded");
                FocusManager.SetFocusedElement(this, SupplierBox);
            };
        }

        private void SupplierBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is HeaderViewModel vm && sender is ComboBox combo)
            {
                vm.EnsureSupplierExists(combo.Text);
            }
        }
    }
}
