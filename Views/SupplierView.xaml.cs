using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Helpers;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Views
{
    public partial class SupplierView : UserControl
    {
        private SupplierViewModel ViewModel => (SupplierViewModel)DataContext;

        public SupplierView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
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
            DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
        }
    }
}
