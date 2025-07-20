using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceApp.Shared.Helpers;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class InvoiceItemDataGrid : UserControl
    {
        public InvoiceItemDataGrid()
        {
            InitializeComponent();
            Log.Information("InvoiceItemDataGrid initialized");
        }

        public DataGrid DataGrid => InnerGrid;

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
        }

        private void DataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            if (DataContext is ItemsViewModel vm && e.NewItem is InvoiceItemViewModel item)
            {
                var product = vm.Products.FirstOrDefault();
                var rate = vm.TaxRates.FirstOrDefault();

                item.Item.InvoiceId = vm.CurrentInvoice?.Id ?? 0;
                item.Item.Product = product;
                item.ProductId = product?.Id ?? 0;
                item.Item.TaxRate = rate;
                item.TaxRateId = rate?.Id ?? 0;
                item.TaxRatePercentage = rate?.Percentage ?? 0m;
                item.IsGross = vm.IsGross;
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.IsNewItem && sender is DataGrid grid)
            {
                grid.Dispatcher.InvokeAsync(() =>
                {
                    grid.SelectedItem = e.Row.Item;
                    grid.CurrentCell = new DataGridCellInfo(e.Row.Item, grid.Columns[0]);
                    grid.BeginEdit();
                });
            }
        }
    }
}
