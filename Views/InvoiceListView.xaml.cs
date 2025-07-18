using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Views
{
    public partial class InvoiceListView : UserControl
    {
        private InvoiceViewModel ViewModel => (InvoiceViewModel)DataContext;

        public InvoiceListView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                if (DataContext is InvoiceViewModel vm)
                {
                    await vm.LoadAsync();
                    FocusManager.SetFocusedElement(this, InvoicesGrid);
                }
            };
        }
    }
}
