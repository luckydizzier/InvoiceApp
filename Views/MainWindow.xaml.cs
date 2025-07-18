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

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Up)
            {
                if (_viewModel.NavigateUpCommand.CanExecute(null))
                {
                    _viewModel.NavigateUpCommand.Execute(null);
                    e.Handled = true;
                }
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                if (_viewModel.NavigateDownCommand.CanExecute(null))
                {
                    _viewModel.NavigateDownCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
