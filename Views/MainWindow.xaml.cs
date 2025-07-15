using System.Windows;
using InvoiceApp.ViewModels;
using InvoiceApp;
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
                if (DialogHelper.ConfirmDeletion("tételt"))
                {
                    _viewModel.RemoveItemCommand.Execute(item);
                    DialogHelper.ShowInfo("Tétel törölve.");
                }
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

        private void InvoicesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (InvoicesList.SelectedItem != null)
            {
                MainTabs.SelectedIndex = 1;
            }
        }


        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    if (InvoicesList.SelectedIndex > 0)
                    {
                        InvoicesList.SelectedIndex--;
                        InvoicesList.ScrollIntoView(InvoicesList.SelectedItem);
                    }
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Down:
                    if (InvoicesList.SelectedIndex < InvoicesList.Items.Count - 1)
                    {
                        InvoicesList.SelectedIndex++;
                        InvoicesList.ScrollIntoView(InvoicesList.SelectedItem);
                    }
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Enter:
                    if (InvoicesList.SelectedItem != null)
                    {
                        ItemsGrid.Focus();
                    }
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Escape:
                    InvoicesList.Focus();
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Delete:
                    if (InvoicesList.SelectedItem is InvoiceApp.Models.Invoice invoice)
                    {
                        var result = MessageBox.Show("Biztosan törli a számlát?", "Törlés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            var service = ((App)Application.Current).Services.GetRequiredService<InvoiceApp.Services.IInvoiceService>();
                            service.DeleteAsync(invoice.Id).GetAwaiter().GetResult();
                            _viewModel.Invoices.Remove(invoice);
                        }
                        e.Handled = true;
                    }
                    break;
            }
        }
    }
}
