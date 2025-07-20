using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Shared.Helpers;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class TaxRateView : UserControl
    {
        private TaxRateViewModel ViewModel => (TaxRateViewModel)DataContext;

        public TaxRateView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("TaxRateView loaded");
                if (DataContext is TaxRateViewModel vm)
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
                ViewModel.SelectPreviousRate();
                e.Handled = true;
            }
            else
            {
                DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
            }
        }

        private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (DataContext is TaxRateViewModel vm)
            {
                vm.MarkDirty();
            }
        }
    }
}
