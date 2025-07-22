using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using InvoiceApp.Presentation.ViewModels;
using InvoiceApp.Shared;
using Serilog;

namespace InvoiceApp.Presentation.Views
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
            // Allow exiting the application with Escape when on the main view
            if (e.Key == System.Windows.Input.Key.Escape &&
                ViewModel.CurrentState == AppState.MainWindow &&
                !ViewModel.InvoiceViewModel.IsRowDetailsVisible)
            {
                var result = MessageBox.Show("Do you really want to exit?", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
                return;
            }

            // Ignore navigation keys when editing in text fields or combo boxes
            if (e.OriginalSource is TextBoxBase || e.OriginalSource is ComboBox)
            {
                return;
            }

            // Only allow list navigation when the main window list is active and
            // no row details panel is open
            if (ViewModel.CurrentState != Shared.AppState.MainWindow ||
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
            else if (e.Key == System.Windows.Input.Key.Escape &&
                     ViewModel.CurrentState == AppState.Summary)
            {
                if (ViewModel.SummaryEscapeCommand.CanExecute(null))
                {
                    ViewModel.SummaryEscapeCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
