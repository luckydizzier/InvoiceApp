using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Helpers;
using InvoiceApp.ViewModels;
using Serilog;

namespace InvoiceApp.Views
{
    public partial class ProductGroupView : UserControl
    {
        private ProductGroupViewModel ViewModel => (ProductGroupViewModel)DataContext;

        public ProductGroupView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("ProductGroupView loaded");
                if (DataContext is ProductGroupViewModel vm)
                {
                    await vm.LoadAsync();
                    FocusManager.SetFocusedElement(this, DataGrid);
                }
            };
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && sender is DataGrid grid && grid.SelectedIndex == 0)
            {
                ViewModel.SelectPreviousGroup();
                e.Handled = true;
            }
            else
            {
                DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
            }
        }

        private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (DataContext is ProductGroupViewModel vm)
            {
                vm.MarkDirty();
            }
        }
    }
}
