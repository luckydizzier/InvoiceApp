using System.Windows.Controls;

namespace InvoiceApp.Views
{
    public partial class InvoiceItemDataGrid : UserControl
    {
        public InvoiceItemDataGrid()
        {
            InitializeComponent();
        }

        public DataGrid DataGrid => InnerGrid;
    }
}
