using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<MainViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) =>
            {
                await _viewModel.InvoiceViewModel.LoadAsync();
                _viewModel.ShowInvoiceListCommand.Execute(null);
                _viewModel.InvoiceViewModel.IsInvoiceListFocused = true;
            };
        }
    }
}
