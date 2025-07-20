using System.Windows;
using InvoiceApp.Presentation.ViewModels;
using Serilog;

namespace InvoiceApp.Presentation.Views
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialogViewModel ViewModel { get; }

        public ConfirmDialog()
        {
            InitializeComponent();
            Log.Information("ConfirmDialog initialized");
            ViewModel = new ConfirmDialogViewModel(string.Empty, SetResult);
            DataContext = ViewModel;
        }

        public ConfirmDialog(string message) : this()
        {
            ViewModel.Message = message;
        }

        private void SetResult(bool? result)
        {
            DialogResult = result;
        }
    }
}
