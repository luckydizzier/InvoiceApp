using System.Windows.Controls;

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
    }
}
