using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using InvoiceApp.Helpers;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class ProductView : UserControl
    {
        private readonly ProductViewModel _viewModel;

        public ProductView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<ProductViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }

        private void DataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || sender is not DataGrid grid)
            {
                return;
            }

            var cell = FindParent<DataGridCell>(e.OriginalSource as DependencyObject);
            if (cell == null)
            {
                return;
            }

            if (!cell.IsEditing)
            {
                grid.BeginEdit();
            }
            else
            {
                grid.CommitEdit(DataGridEditingUnit.Cell, true);
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                DataGridFocusBehavior.MoveFocus(grid);
            }

            e.Handled = true;
        }

        private static T? FindParent<T>(DependencyObject? child) where T : DependencyObject
        {
            while (child != null && child is not T)
            {
                child = VisualTreeHelper.GetParent(child);
            }

            return child as T;
        }
    }
}
