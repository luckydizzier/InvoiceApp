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
        private IInputElement? _lastFocused;
        private IInputElement? _focusBeforeList;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<InvoiceViewModel>();
            DataContext = _viewModel;
            PreviewGotKeyboardFocus += (s, e) =>
            {
                if (e.Source == InvoicesList)
                {
                    if (_focusBeforeList == null)
                    {
                        _focusBeforeList = _lastFocused;
                    }
                }
                else
                {
                    _lastFocused = (IInputElement)e.Source;
                }
            };
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
            _viewModel.NewInvoiceCommand.Execute(null);
            _viewModel.IsInvoiceListFocused = true;
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
                    if (InvoicesList.IsKeyboardFocusWithin)
                    {
                        if (InvoicesList.Items.Count == 0 || InvoicesList.SelectedIndex == 0)
                        {
                            _viewModel.NewInvoiceCommand.Execute(null);
                        }
                        _lastFocused = ItemsGrid;
                        ItemsGrid.Focus();
                        if (ItemsGrid.Items.Count > 0)
                        {
                            ItemsGrid.CurrentCell = new DataGridCellInfo(ItemsGrid.Items[0], ItemsGrid.Columns[0]);
                        }
                        e.Handled = true;
                    }
                    else if (!ItemsGrid.IsKeyboardFocusWithin && InvoicesList.SelectedItem != null)
                    {
                        _lastFocused = ItemsGrid;
                        ItemsGrid.Focus();
                        if (ItemsGrid.Items.Count > 0)
                        {
                            ItemsGrid.CurrentCell = new DataGridCellInfo(ItemsGrid.Items[0], ItemsGrid.Columns[0]);
                        }
                        e.Handled = true;
                    }
                    break;
                case System.Windows.Input.Key.Escape:
                    if (InvoicesList.IsKeyboardFocusWithin)
                    {
                        if (_focusBeforeList != null)
                        {
                            (_focusBeforeList as Control)?.Focus();
                            _focusBeforeList = null;
                        }
                    }
                    else
                    {
                        _focusBeforeList = System.Windows.Input.Keyboard.FocusedElement;
                        InvoicesList.Focus();
                    }
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Delete:
                    if (InvoicesList.IsKeyboardFocusWithin && InvoicesList.SelectedItem is InvoiceApp.Models.Invoice invoice)
                    {
                        if (DialogHelper.ConfirmDeletion("számlát"))
                        {
                            _viewModel.RemoveInvoiceCommand.Execute(invoice);
                            DialogHelper.ShowInfo("Számla törölve.");
                        }
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
                var lastColumn = ItemsGrid.Columns.Count - 2;
                if (ItemsGrid.SelectedIndex == ItemsGrid.Items.Count - 1 && index == lastColumn)
                {
                    _viewModel.SaveItemCommand.Execute(item);
                    _viewModel.AddItemCommand.Execute(null);
                    ItemsGrid.SelectedIndex = ItemsGrid.Items.Count - 1;
                    ItemsGrid.CurrentCell = new DataGridCellInfo(ItemsGrid.SelectedItem, ItemsGrid.Columns[0]);
                }
                else if (index < lastColumn)
                {
                    ItemsGrid.CurrentCell = new DataGridCellInfo(item, ItemsGrid.Columns[index + 1]);
                }
                else
                {
                    _viewModel.SaveItemCommand.Execute(item);
                }
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Delete && ItemsGrid.SelectedItem is InvoiceItemViewModel delItem)
            {
                if (DialogHelper.ConfirmDeletion("tételt"))
                {
                    _viewModel.RemoveItemCommand.Execute(delItem);
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
