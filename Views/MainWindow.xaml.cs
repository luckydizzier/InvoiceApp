using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
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

        private DataGrid ItemsDataGrid => ItemsGrid.DataGrid;
        public ICommand NavigateUpCommand { get; }
        public ICommand NavigateDownCommand { get; }
        public ICommand EnterCommand { get; }
        public ICommand EscapeCommand { get; }
        public ICommand DeleteInvoiceCommand { get; }
        public ICommand ItemsEnterCommand { get; }
        public ICommand ItemsDeleteCommand { get; }
        public ICommand OpenPaymentMethodsCommand { get; }
        public ICommand OpenSuppliersCommand { get; }
        public ICommand OpenProductGroupsCommand { get; }
        public ICommand OpenTaxRatesCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<InvoiceViewModel>();
            DataContext = _viewModel;
            NavigateUpCommand = new RelayCommand(_ => NavigateUp());
            NavigateDownCommand = new RelayCommand(_ => NavigateDown());
            EnterCommand = new RelayCommand(async _ => await EnterKey());
            EscapeCommand = new RelayCommand(_ => EscapeKey());
            DeleteInvoiceCommand = new RelayCommand(_ => DeleteInvoice());
            ItemsEnterCommand = new RelayCommand(_ => ItemsEnter());
            ItemsDeleteCommand = new RelayCommand(_ => ItemsDelete());
            OpenPaymentMethodsCommand = new RelayCommand(_ => ShowPaymentMethods());
            OpenSuppliersCommand = new RelayCommand(_ => ShowSuppliers());
            OpenProductGroupsCommand = new RelayCommand(_ => ShowProductGroups());
            OpenTaxRatesCommand = new RelayCommand(_ => ShowTaxRates());
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
            await _viewModel.NewInvoice();
            _viewModel.IsInvoiceListFocused = true;
        }


        private void ShowPaymentMethods()
        {
            var win = new PaymentMethodView();
            win.ShowDialog();
        }

        private void OpenPaymentMethods(object? sender, RoutedEventArgs? e) => ShowPaymentMethods();

        private void ShowSuppliers()
        {
            var win = new SupplierView();
            win.ShowDialog();
        }

        private void OpenSuppliers(object? sender, RoutedEventArgs? e) => ShowSuppliers();

        private void ShowProductGroups()
        {
            var win = new ProductGroupView();
            win.ShowDialog();
        }

        private void OpenProductGroups(object? sender, RoutedEventArgs? e) => ShowProductGroups();

        private void ShowTaxRates()
        {
            var win = new TaxRateView();
            win.ShowDialog();
        }

        private void OpenTaxRates(object? sender, RoutedEventArgs? e) => ShowTaxRates();

        private void NavigateUp()
        {
            if (!InvoicesList.IsKeyboardFocusWithin)
            {
                return;
            }

            if (InvoicesList.SelectedIndex > 0)
            {
                InvoicesList.SelectedIndex--;
                InvoicesList.ScrollIntoView(InvoicesList.SelectedItem);
            }
        }

        private void NavigateDown()
        {
            if (!InvoicesList.IsKeyboardFocusWithin)
            {
                return;
            }

            if (InvoicesList.SelectedIndex < InvoicesList.Items.Count - 1)
            {
                InvoicesList.SelectedIndex++;
                InvoicesList.ScrollIntoView(InvoicesList.SelectedItem);
            }
        }

        private async Task EnterKey()
        {
            if (InvoicesList.IsKeyboardFocusWithin)
            {
                if (InvoicesList.Items.Count == 0 || InvoicesList.SelectedIndex == 0)
                {
                    await _viewModel.NewInvoice();
                }
                _lastFocused = ItemsDataGrid;
                ItemsDataGrid.Focus();
                if (ItemsDataGrid.Items.Count > 0)
                {
                    ItemsDataGrid.CurrentCell = new DataGridCellInfo(ItemsDataGrid.Items[0], ItemsDataGrid.Columns[0]);
                }
            }
            else if (!ItemsDataGrid.IsKeyboardFocusWithin && InvoicesList.SelectedItem != null)
            {
                _lastFocused = ItemsDataGrid;
                ItemsDataGrid.Focus();
                if (ItemsDataGrid.Items.Count > 0)
                {
                    ItemsDataGrid.CurrentCell = new DataGridCellInfo(ItemsDataGrid.Items[0], ItemsDataGrid.Columns[0]);
                }
            }
        }

        private void EscapeKey()
        {
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
                _focusBeforeList = Keyboard.FocusedElement;
                InvoicesList.Focus();
            }
        }

        private void DeleteInvoice()
        {
            if (InvoicesList.IsKeyboardFocusWithin && InvoicesList.SelectedItem is InvoiceApp.Models.Invoice invoice)
            {
                if (DialogHelper.ConfirmDeletion("számlát"))
                {
                    _viewModel.RemoveInvoiceCommand.Execute(invoice);
                    DialogHelper.ShowInfo("Számla törölve.");
                }
            }
        }

        private void ItemsEnter()
        {
            if (ItemsDataGrid.CurrentCell != null && ItemsDataGrid.SelectedItem is InvoiceItemViewModel item)
            {
                ItemsDataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                ItemsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                var index = ItemsDataGrid.Columns.IndexOf(ItemsDataGrid.CurrentCell.Column);
                var lastColumn = ItemsDataGrid.Columns.Count - 2;
                if (ItemsDataGrid.SelectedIndex == ItemsDataGrid.Items.Count - 1 && index == lastColumn)
                {
                    _viewModel.SaveItemCommand.Execute(item);
                    _viewModel.AddItemCommand.Execute(null);
                    ItemsDataGrid.SelectedIndex = ItemsDataGrid.Items.Count - 1;
                    ItemsDataGrid.CurrentCell = new DataGridCellInfo(ItemsDataGrid.SelectedItem, ItemsDataGrid.Columns[0]);
                }
                else if (index < lastColumn)
                {
                    ItemsDataGrid.CurrentCell = new DataGridCellInfo(item, ItemsDataGrid.Columns[index + 1]);
                }
                else
                {
                    _viewModel.SaveItemCommand.Execute(item);
                }
            }
        }

        private void ItemsDelete()
        {
            if (ItemsDataGrid.SelectedItem is InvoiceItemViewModel delItem)
            {
                if (DialogHelper.ConfirmDeletion("tételt"))
                {
                    _viewModel.RemoveItemCommand.Execute(delItem);
                }
            }
        }



    }
}
