using System.Threading.Tasks;
using System.Collections.ObjectModel;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly IInvoiceService _service;
        private ObservableCollection<Invoice> _invoices = new();

        public ObservableCollection<Invoice> Invoices
        {
            get => _invoices;
            set
            {
                _invoices = value;
                OnPropertyChanged();
            }
        }

        public InvoiceViewModel(IInvoiceService service)
        {
            _service = service;
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Invoices = new ObservableCollection<Invoice>(items);
        }
    }
}
