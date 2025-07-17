using System.Windows;
using System.Windows.Input;

namespace InvoiceApp.Views
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialog()
        {
            InitializeComponent();
        }

        public ConfirmDialog(string message) : this()
        {
            Message = message;
        }

        public string Message
        {
            get => MessageText.Text;
            set => MessageText.Text = value;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}
