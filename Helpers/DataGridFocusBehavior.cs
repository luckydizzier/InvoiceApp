using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InvoiceApp.Helpers
{
    public static class DataGridFocusBehavior
    {
        public static void MoveFocus(DataGrid grid)
        {
            int columnIndex = grid.Columns.IndexOf(grid.CurrentColumn);
            int rowIndex = grid.Items.IndexOf(grid.CurrentItem);

            columnIndex++;
            if (columnIndex >= grid.Columns.Count)
            {
                columnIndex = 0;
                if (rowIndex < grid.Items.Count - 1)
                {
                    rowIndex++;
                }
            }

            if (rowIndex >= 0 && rowIndex < grid.Items.Count)
            {
                grid.CurrentCell = new DataGridCellInfo(grid.Items[rowIndex], grid.Columns[columnIndex]);
                grid.SelectedIndex = rowIndex;
                grid.Dispatcher.InvokeAsync(() => grid.BeginEdit());
            }
        }

        public static void OnPreviewKeyDown(object sender, KeyEventArgs e)
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
                MoveFocus(grid);
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
