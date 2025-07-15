using System.Windows;
using InvoiceApp.ViewModels;
using InvoiceApp;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace InvoiceApp.Views
{
    public partial class InvoiceEditorView : Window
    {
        private readonly InvoiceViewModel _viewModel;

        public InvoiceEditorView()
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<InvoiceViewModel>();
            DataContext = _viewModel;
        }

        private void AddItemClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.AddItemCommand.Execute(null);
        }

        private void DeleteItemClicked(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is InvoiceItemViewModel item)
            {
                if (DialogHelper.ConfirmDeletion("tételt"))
                {
                    _viewModel.RemoveItemCommand.Execute(item);
                    DialogHelper.ShowInfo("Tétel törölve.");
                }
            }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }
    }
}
