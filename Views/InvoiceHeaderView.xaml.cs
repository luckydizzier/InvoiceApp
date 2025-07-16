using System.Windows;
using System.Windows.Controls;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Views
{
    public partial class InvoiceHeaderView : UserControl
    {
        public InvoiceHeaderView()
        {
            InitializeComponent();
        }

        private void SupplierBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is InvoiceViewModel vm && sender is ComboBox combo)
            {
                vm.EnsureSupplierExists(combo.Text);
            }
        }
    }
}
