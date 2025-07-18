using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class InvoiceListView : UserControl
    {
        private readonly InvoiceViewModel _viewModel;

        public InvoiceListView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<InvoiceViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) =>
            {
                await _viewModel.LoadAsync();
                FocusManager.SetFocusedElement(this, InvoicesGrid);
            };
        }
    }
}
