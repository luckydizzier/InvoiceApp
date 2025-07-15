using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly InvoiceViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<InvoiceViewModel>();
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

        private void AddItemClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.AddItemCommand.Execute(null);
        }

        private void DeleteItemClicked(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is InvoiceItemViewModel item)
            {
                _viewModel.RemoveItemCommand.Execute(item);
            }
        }

        private void OpenPaymentMethods(object sender, RoutedEventArgs e)
        {
            var win = new PaymentMethodView();
            win.ShowDialog();
        }

        private void OpenSuppliers(object sender, RoutedEventArgs e)
        {
            var win = new SupplierView();
            win.ShowDialog();
        }

        private void OpenProductGroups(object sender, RoutedEventArgs e)
        {
            var win = new ProductGroupView();
            win.ShowDialog();
        }

        private void OpenTaxRates(object sender, RoutedEventArgs e)
        {
            var win = new TaxRateView();
            win.ShowDialog();
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                var result = MessageBox.Show("Biztosan kilép?", "Kilépés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
                e.Handled = true;
            }
        }
    }
}
