using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Shared.Helpers;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class ProductView : UserControl
    {
        private ProductViewModel ViewModel => (ProductViewModel)DataContext;

        public ProductView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("ProductView loaded");
                if (DataContext is ProductViewModel vm)
                {
                    await vm.LoadAsync();
                    FocusManager.SetFocusedElement(this, SearchBox);
                }
            };
        }

        private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (DataContext is ProductViewModel vm)
            {
                vm.MarkDirty();
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && sender is DataGrid grid && grid.SelectedIndex == 0)
            {
                ViewModel.SelectPreviousProduct();
                e.Handled = true;
            }
            else
            {
                DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
            }
        }
    }
}
