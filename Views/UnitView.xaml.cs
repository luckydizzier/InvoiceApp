using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Helpers;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Views
{
    public partial class UnitView : UserControl
    {
        private UnitViewModel ViewModel => (UnitViewModel)DataContext;

        public UnitView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                if (DataContext is UnitViewModel vm)
                {
                    await vm.LoadAsync();
                    FocusManager.SetFocusedElement(this, DataGrid);
                }
            };
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
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
