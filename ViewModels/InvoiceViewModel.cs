using System.Threading.Tasks;
using System.Collections.ObjectModel;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly IInvoiceService _service;
        private readonly IChangeLogService _logService;
        private ObservableCollection<Invoice> _invoices = new();
        private string _statusMessage = string.Empty;

        public ObservableCollection<Invoice> Invoices
        {
            get => _invoices;
            set
            {
                _invoices = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public InvoiceViewModel(IInvoiceService service, IChangeLogService logService)
        {
            _service = service;
            _logService = logService;
        }

        public async Task LoadAsync()
        {
            StatusMessage = "Betöltés...";
            var items = await _service.GetAllAsync();
            Invoices = new ObservableCollection<Invoice>(items);

            var log = await _logService.GetLatestAsync();
            if (log != null)
            {
                StatusMessage = $"Utolsó esemény: {log.Operation} ({log.DateCreated:g})";
            }
            else
            {
                StatusMessage = Invoices.Count == 0 ? "Üres lista." : $"{Invoices.Count} számla betöltve.";
            }
        }
    }
}
