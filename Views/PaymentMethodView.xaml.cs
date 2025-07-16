using System.Windows.Controls;
using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class PaymentMethodView : UserControl
    {
        private readonly PaymentMethodViewModel _viewModel;

        public PaymentMethodView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<PaymentMethodViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }
    }
}
