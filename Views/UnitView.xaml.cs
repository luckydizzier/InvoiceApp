using System.Windows;
using System.Windows.Input;
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }
    }
}
