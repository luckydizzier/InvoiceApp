using System;

namespace InvoiceApp.ViewModels
{
    public class ConfirmDialogViewModel : ViewModelBase
    {
        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public RelayCommand YesCommand { get; }
        public RelayCommand NoCommand { get; }

        public ConfirmDialogViewModel(string message, Action<bool?> closeAction)
        {
            _message = message;
            YesCommand = new RelayCommand(_ => closeAction(true));
            NoCommand = new RelayCommand(_ => closeAction(false));
        }
    }
}
