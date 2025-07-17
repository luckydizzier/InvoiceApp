using System.Windows;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Views
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialogViewModel ViewModel { get; }

        public ConfirmDialog()
        {
            InitializeComponent();
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
