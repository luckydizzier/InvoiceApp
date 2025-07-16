using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class TaxRateView : Window
    {
        private readonly TaxRateViewModel _viewModel;

        public TaxRateView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<TaxRateViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                if (DataGrid.IsKeyboardFocusWithin)
                {
                    var row = DataGrid.ItemContainerGenerator.ContainerFromItem(DataGrid.CurrentItem) as DataGridRow;
                    if (row != null && row.IsEditing)
                    {
                        DataGrid.CancelEdit();
                        e.Handled = true;
                        return;
                    }
                }
                Close();
                e.Handled = true;
            }
        }
    }
}
