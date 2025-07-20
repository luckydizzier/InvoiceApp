using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using InvoiceApp.ViewModels;
using Serilog;

namespace InvoiceApp.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                Log.Information("MainWindow loaded");
                if (DataContext is MainViewModel vm)
                {
                    await vm.InvoiceViewModel.LoadAsync();
                    vm.InvoiceViewModel.IsInvoiceListFocused = true;
                }
            };
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Ignore navigation keys when editing in text fields or combo boxes
            if (e.OriginalSource is TextBoxBase || e.OriginalSource is ComboBox)
            {
                return;
            }

            // Only allow list navigation when the main window list is active and
            // no row details panel is open
            if (ViewModel.CurrentState != Models.AppState.MainWindow ||
                ViewModel.InvoiceViewModel.IsRowDetailsVisible)
            {
                return;
            }

            if (e.Key == System.Windows.Input.Key.Up)
            {
                if (ViewModel.NavigateUpCommand.CanExecute(null))
                {
                    ViewModel.NavigateUpCommand.Execute(null);
                    e.Handled = true;
                }
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                if (ViewModel.NavigateDownCommand.CanExecute(null))
                {
                    ViewModel.NavigateDownCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
