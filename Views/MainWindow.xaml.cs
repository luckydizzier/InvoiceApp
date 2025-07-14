using System.Windows;
using InvoiceApp.ViewModels;

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
            _viewModel = new InvoiceViewModel(new Services.InvoiceService(new Repositories.MockInvoiceRepository()));
            DataContext = _viewModel;
            Loaded += async (_, __) => await _viewModel.LoadAsync();
        }
    }
}
