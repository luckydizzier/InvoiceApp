using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class DashboardView : UserControl
    {
        private DashboardViewModel ViewModel => (DashboardViewModel)DataContext;

        public DashboardView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("DashboardView loaded");
                if (DataContext is DashboardViewModel vm)
                {
                    await vm.LoadAsync();
                    FocusManager.SetFocusedElement(this, MenuList);
                }
            };
        }
    }
}
