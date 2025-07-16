using System.Windows;
using InvoiceApp.Views;

namespace InvoiceApp
{
    public static class DialogHelper
    {
        public static bool ConfirmDeletion(string itemName)
        {
            var message = $"Biztosan törli a(z) {itemName} elemet?";
            return ShowConfirmation(message, "Megerősítés");
        }

        public static bool ShowConfirmation(string message, string title)
        {
            var dialog = new ConfirmDialog(message) { Title = title };
            return dialog.ShowDialog() == true;
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
