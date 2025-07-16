using System.Windows.Controls;
using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class TaxRateView : UserControl
    {
        private readonly TaxRateViewModel _viewModel;

        public TaxRateView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<TaxRateViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }
    }
}
