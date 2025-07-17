using System.Windows.Controls;
using System.Windows.Input;
using InvoiceApp.Helpers;

namespace InvoiceApp.Views
{
    public partial class InvoiceItemDataGrid : UserControl
    {
        public InvoiceItemDataGrid()
        {
            InitializeComponent();
        }

        public DataGrid DataGrid => InnerGrid;

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
        }
    }
}
