using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Views
{
    public partial class DashboardView : UserControl
    {
        private DashboardViewModel ViewModel => (DashboardViewModel)DataContext;

        public DashboardView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                if (DataContext is DashboardViewModel vm)
                {
                    await vm.LoadAsync();
                    FocusManager.SetFocusedElement(this, MenuList);
                }
            };
        }
    }
}
