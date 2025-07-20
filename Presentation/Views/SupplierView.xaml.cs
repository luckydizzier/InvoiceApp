using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Shared.Helpers;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class SupplierView : UserControl
    {
        private SupplierViewModel ViewModel => (SupplierViewModel)DataContext;

        public SupplierView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("SupplierView loaded");
                if (DataContext is SupplierViewModel vm)
                {
                    await vm.LoadAsync();
                    FocusManager.SetFocusedElement(this, DataGrid);
                }
            };
        }

        private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (DataContext is SupplierViewModel vm)
            {
                vm.MarkDirty();
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && sender is DataGrid grid && grid.SelectedIndex == 0)
            {
                ViewModel.SelectPreviousSupplier();
                e.Handled = true;
            }
            else
            {
                DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
            }
        }
    }
}
