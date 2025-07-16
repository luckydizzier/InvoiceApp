using System.Windows;
using System.Windows.Controls;
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
            _viewModel.NewInvoiceCommand.Execute(null);
            this.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
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
                case System.Windows.Input.Key.N:
                    if ((System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
                    {
                        _viewModel.NewInvoiceCommand.Execute(null);
                        ItemsGrid.Focus();
                        e.Handled = true;
                    }
                    break;
                case System.Windows.Input.Key.S:
                    if ((System.Windows.Input.Keyboard.Modifiers & (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)) == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift))
                    {
                        _viewModel.AddSupplierCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void ItemsGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && ItemsGrid.CurrentCell != null && ItemsGrid.SelectedItem is InvoiceItemViewModel item)
            {
                ItemsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                ItemsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                var index = ItemsGrid.Columns.IndexOf(ItemsGrid.CurrentCell.Column);
                if (index < ItemsGrid.Columns.Count - 2)
                {
                    ItemsGrid.CurrentCell = new DataGridCellInfo(item, ItemsGrid.Columns[index + 1]);
                }
                else
                {
                    _viewModel.SaveItemCommand.Execute(item);
                }
                e.Handled = true;
            }
        }

        private void ItemsGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = _viewModel.CreateItemViewModel();
        }
    }
}
