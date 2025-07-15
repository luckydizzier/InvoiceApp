using System.Windows;
using System.Windows.Input;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class ProductView : Window
    {
        private readonly ProductViewModel _viewModel;

        public ProductView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<ProductViewModel>();
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
