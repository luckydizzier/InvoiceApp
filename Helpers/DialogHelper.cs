using System.Windows;

namespace InvoiceApp
{
    public static class DialogHelper
    {
        public static bool ConfirmDeletion(string itemName)
        {
            var message = $"Biztosan törli a(z) {itemName} elemet?";
            var result = MessageBox.Show(message, "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
