using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public ICommand NavigateUpCommand { get; }
        public ICommand NavigateDownCommand { get; }
        public ICommand EnterCommand { get; }
        public ICommand EscapeCommand { get; }
        public ICommand DeleteInvoiceCommand { get; }
        public ICommand ItemsEnterCommand { get; }
        public ICommand ItemsDeleteCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<InvoiceViewModel>();
            DataContext = _viewModel;
            NavigateUpCommand = new RelayCommand(_ => NavigateUp());
            NavigateDownCommand = new RelayCommand(_ => NavigateDown());
            EnterCommand = new RelayCommand(_ => EnterKey());
            EscapeCommand = new RelayCommand(_ => EscapeKey());
            DeleteInvoiceCommand = new RelayCommand(_ => DeleteInvoice());
            ItemsEnterCommand = new RelayCommand(_ => ItemsEnter());
            ItemsDeleteCommand = new RelayCommand(_ => ItemsDelete());
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

        private void NavigateUp()
        {
            if (InvoicesList.SelectedIndex > 0)
            {
                InvoicesList.SelectedIndex--;
                InvoicesList.ScrollIntoView(InvoicesList.SelectedItem);
            }
        }

        private void NavigateDown()
        {
            if (InvoicesList.SelectedIndex < InvoicesList.Items.Count - 1)
            {
                InvoicesList.SelectedIndex++;
                InvoicesList.ScrollIntoView(InvoicesList.SelectedItem);
            }
        }

        private void EnterKey()
        {
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
            }
            else if (!ItemsGrid.IsKeyboardFocusWithin && InvoicesList.SelectedItem != null)
            {
                _lastFocused = ItemsGrid;
                ItemsGrid.Focus();
                if (ItemsGrid.Items.Count > 0)
                {
                    ItemsGrid.CurrentCell = new DataGridCellInfo(ItemsGrid.Items[0], ItemsGrid.Columns[0]);
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
            if (ItemsGrid.CurrentCell != null && ItemsGrid.SelectedItem is InvoiceItemViewModel item)
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
            }
        }

        private void ItemsDelete()
        {
            if (ItemsGrid.SelectedItem is InvoiceItemViewModel delItem)
            {
                if (DialogHelper.ConfirmDeletion("tételt"))
                {
                    _viewModel.RemoveItemCommand.Execute(delItem);
                }
            }
        }



    }
}
