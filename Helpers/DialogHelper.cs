using System.Windows;
using System.Linq;
using InvoiceApp.Views;

namespace InvoiceApp
{
    public static class DialogHelper
    {
        /// <summary>
        /// Optional handler used in tests to override confirmation dialogs.
        /// </summary>
        public static Func<string, string, bool>? ConfirmationHandler { get; set; }
        public static bool ConfirmDeletion(string itemName)
        {
            var message = $"Biztosan törli a(z) {itemName} elemet?";
            return ShowConfirmation(message, "Megerősítés");
        }

        public static bool ShowConfirmation(string message, string title)
        {
            if (ConfirmationHandler != null)
            {
                return ConfirmationHandler(message, title);
            }
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

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
