using System.Windows;
using System.Windows.Input;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class TaxRateView : Window
    {
        private readonly TaxRateViewModel _viewModel;

        public TaxRateView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<TaxRateViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }
    }
}
