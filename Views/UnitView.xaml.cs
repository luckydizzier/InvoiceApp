using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Views
{
    public partial class UnitView : Window
    {
        private readonly UnitViewModel _viewModel;

        public UnitView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<UnitViewModel>();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadAsync();
        }
    }
}
