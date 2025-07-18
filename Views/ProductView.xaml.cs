using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
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
            Loaded += async (s, e) =>
            {
                await _viewModel.LoadAsync();
                FocusManager.SetFocusedElement(this, SearchBox);
            };
        }

        private void DataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            _viewModel.MarkDirty();
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGridFocusBehavior.OnPreviewKeyDown(sender, e);
        }
    }
}
