using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class ProductGroupView : Window
    {
        private readonly ProductGroupViewModel _viewModel;

        public ProductGroupView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<ProductGroupViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }
    }
}
