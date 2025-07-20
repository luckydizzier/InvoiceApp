using InvoiceApp.Presentation.ViewModels;

namespace InvoiceApp.Application.Services
{
    /// <summary>
    /// Default implementation of <see cref="IStatusService"/>.
    /// Stores the last shown message and notifies listeners when it changes.
    /// </summary>
    public class StatusService : ViewModelBase, IStatusService
    {
        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            private set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public void Show(string message)
        {
            Message = message;
        }
    }
}
