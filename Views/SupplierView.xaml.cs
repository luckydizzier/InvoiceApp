using System.Windows.Controls;
using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class SupplierView : UserControl
    {
        private readonly SupplierViewModel _viewModel;

        public SupplierView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<SupplierViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }
    }
}
