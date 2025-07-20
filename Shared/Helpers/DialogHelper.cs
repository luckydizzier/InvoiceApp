using System.Windows;
using System.Linq;
using InvoiceApp.Presentation.Views;
using InvoiceApp.Resources;

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
            var message = string.Format(Resources.Strings.ConfirmDeletionFormat, itemName);
            return ShowConfirmation(message, Resources.Strings.ConfirmationTitle);
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
            MessageBox.Show(message, Resources.Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
