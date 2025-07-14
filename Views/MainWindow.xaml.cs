using System.Windows;
using InvoiceApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace InvoiceApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly InvoiceViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<InvoiceViewModel>();

            DataContext = _viewModel;
            Loaded += async (_, __) => await _viewModel.LoadAsync();
        }
    }
}
