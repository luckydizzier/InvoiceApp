using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Helpers;
using InvoiceApp.ViewModels;
using Serilog;

namespace InvoiceApp.Views
{
    public partial class PaymentMethodView : UserControl
    {
        private PaymentMethodViewModel ViewModel => (PaymentMethodViewModel)DataContext;

        public PaymentMethodView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("PaymentMethodView loaded");
                if (DataContext is PaymentMethodViewModel vm)
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
                ViewModel.SelectPreviousMethod();
                e.Handled = true;
            }
            else
            {
                DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
            }
        }

        private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (DataContext is PaymentMethodViewModel vm)
            {
                vm.MarkDirty();
            }
        }
    }
}
