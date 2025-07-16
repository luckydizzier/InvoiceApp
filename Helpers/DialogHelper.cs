using System.Windows;
using System.Linq;
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
            var owner = Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive);
            var dialog = new ConfirmDialog(message)
            {
                Title = title,
                Owner = owner
            };
            return dialog.ShowDialog() == true;
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
