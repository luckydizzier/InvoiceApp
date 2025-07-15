using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly IInvoiceService _service;
        private readonly IInvoiceItemService _itemService;
        private readonly IProductService _productService;
        private readonly IChangeLogService _logService;
        private ObservableCollection<Invoice> _invoices = new();
        private ObservableCollection<InvoiceItem> _items = new();
        private ObservableCollection<Product> _products = new();
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

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SaveCommand { get; }

        public InvoiceViewModel(IInvoiceService service,
            IInvoiceItemService itemService,
            IProductService productService,
            IChangeLogService logService)
        {
            _service = service;
            _itemService = itemService;
            _productService = productService;
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

            var prods = await _productService.GetAllAsync();
            Products = new ObservableCollection<Product>(prods);

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
            var firstProduct = Products.FirstOrDefault();
            var newItem = new InvoiceItem
            {
                InvoiceId = SelectedInvoice.Id,
                Quantity = 1,
                Product = firstProduct,
                ProductId = firstProduct?.Id ?? 0
            };
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
                if (it.Product != null)
                {
                    await _productService.SaveAsync(it.Product);
                }
                await _itemService.SaveAsync(it);
            }

            StatusMessage = $"Számla mentve. ({DateTime.Now:g})";
        }
    }
}
