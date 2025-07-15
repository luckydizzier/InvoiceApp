using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly IInvoiceService _service;
        private readonly IInvoiceItemService _itemService;
        private readonly IChangeLogService _logService;
        private ObservableCollection<Invoice> _invoices = new();
        private ObservableCollection<InvoiceItem> _items = new();
        private Invoice? _selectedInvoice;
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

        public Invoice? SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                _selectedInvoice = value;
                Items = value != null ? new ObservableCollection<InvoiceItem>(value.Items) : new ObservableCollection<InvoiceItem>();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<InvoiceItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SaveCommand { get; }

        public InvoiceViewModel(IInvoiceService service, IInvoiceItemService itemService, IChangeLogService logService)
        {
            _service = service;
            _itemService = itemService;
            _logService = logService;

            AddItemCommand = new RelayCommand(_ => AddItem());
            RemoveItemCommand = new RelayCommand(obj =>
            {
                if (obj is InvoiceItem item)
                {
                    RemoveItem(item);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
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

        private void AddItem()
        {
            if (SelectedInvoice == null) return;
            var newItem = new InvoiceItem { InvoiceId = SelectedInvoice.Id, Quantity = 1 };
            Items.Add(newItem);
        }

        private void RemoveItem(InvoiceItem item)
        {
            Items.Remove(item);
        }

        private async Task SaveAsync()
        {
            if (SelectedInvoice == null) return;

            SelectedInvoice.Items = new List<InvoiceItem>(Items);
            await _service.SaveAsync(SelectedInvoice);

            foreach (var it in Items)
            {
                await _itemService.SaveAsync(it);
            }

            StatusMessage = $"Számla mentve. ({DateTime.Now:g})";
        }
    }
}
