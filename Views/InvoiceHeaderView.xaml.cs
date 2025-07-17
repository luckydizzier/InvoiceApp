using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void HeaderView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var element = Keyboard.FocusedElement as UIElement;
                bool isLast = element is CheckBox;
                element?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                if (isLast)
                {
                    if (Window.GetWindow(this)?.DataContext is MainViewModel vm)
                    {
                        vm.HeaderEnterCommand.Execute(null);
                    }
                }

                e.Handled = true;
            }
        }
    }
}
