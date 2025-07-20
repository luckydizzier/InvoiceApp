using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Shared.Helpers;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class UnitView : UserControl
    {
        private UnitViewModel ViewModel => (UnitViewModel)DataContext;

        public UnitView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("UnitView loaded");
                if (DataContext is UnitViewModel vm)
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
                ViewModel.SelectPreviousUnit();
                e.Handled = true;
            }
            else
            {
                DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
            }
        }

        private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (DataContext is UnitViewModel vm)
            {
                vm.MarkDirty();
            }
        }
    }
}
