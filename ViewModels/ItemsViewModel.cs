using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    /// <summary>
    /// View model for managing invoice item list and totals.
    /// </summary>
    public class ItemsViewModel : ViewModelBase
    {
        private readonly IInvoiceItemService _itemService;
        private readonly IProductService _productService;
        private readonly ITaxRateService _taxRateService;
        private readonly IStatusService _statusService;
        private readonly IInvoiceService _invoiceService;
        private readonly Action _raiseSaveChanged;
        private readonly Action _markDirty;
        private readonly Func<bool> _isGrossFunc;
        private readonly Func<Invoice?> _currentInvoice;

        private ObservableCollection<InvoiceItemViewModel> _items = new();
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<TaxRate> _taxRates = new();
        private ObservableCollection<VatBreakdownEntry> _vatBreakdown = new();
        private InvoiceItemViewModel? _selectedItem;
        private bool _isRowDetailsVisible;
        private decimal _totalNet;
        private decimal _totalVat;
        private decimal _totalGross;
        private string _inWords = string.Empty;

        public Invoice? CurrentInvoice => _currentInvoice();
        public bool IsGross => _isGrossFunc();

        public ItemsViewModel(IInvoiceItemService itemService,
            IProductService productService,
            ITaxRateService taxRateService,
            IInvoiceService invoiceService,
            IStatusService statusService,
            Action raiseSaveChanged,
            Action markDirty,
            Func<bool> isGrossFunc,
            Func<Invoice?> currentInvoice)
        {
            _itemService = itemService;
            _productService = productService;
            _taxRateService = taxRateService;
            _invoiceService = invoiceService;
            _statusService = statusService;
            _raiseSaveChanged = raiseSaveChanged;
            _markDirty = markDirty;
            _isGrossFunc = isGrossFunc;
            _currentInvoice = currentInvoice;

            AddItemCommand = new RelayCommand(_ => AddItem());
            RemoveItemCommand = new RelayCommand(obj =>
            {
                if (obj is InvoiceItemViewModel item && DialogHelper.ConfirmDeletion("tételt"))
                {
                    RemoveItem(item);
                    _statusService.Show("Tétel törölve.");
                }
            });
            SaveItemCommand = new RelayCommand(async obj =>
            {
                if (obj is InvoiceItemViewModel item)
                {
                    await SaveItemAsync(item);
                }
            }, obj => obj is InvoiceItemViewModel);
            NewItemCommand = CreateItemViewModel;
        }

        public ObservableCollection<InvoiceItemViewModel> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                {
                    foreach (var it in _items)
                    {
                        it.PropertyChanged -= Item_PropertyChanged;
                    }
                    _items.CollectionChanged -= Items_CollectionChanged;
                }
                _items = value;
                foreach (var it in _items)
                {
                    it.PropertyChanged += Item_PropertyChanged;
                    it.IsGross = _isGrossFunc();
                }
                _items.CollectionChanged += Items_CollectionChanged;
                OnPropertyChanged();
                UpdateTotals();
                _raiseSaveChanged();
            }
        }

        public InvoiceItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TaxRate> TaxRates
        {
            get => _taxRates;
            set { _taxRates = value; OnPropertyChanged(); }
        }

        public ObservableCollection<VatBreakdownEntry> VatBreakdown
        {
            get => _vatBreakdown;
            set { _vatBreakdown = value; OnPropertyChanged(); }
        }

        public bool IsRowDetailsVisible
        {
            get => _isRowDetailsVisible;
            set { _isRowDetailsVisible = value; OnPropertyChanged(); }
        }

        public decimal TotalNet { get => _totalNet; set { _totalNet = value; OnPropertyChanged(); } }
        public decimal TotalVat { get => _totalVat; set { _totalVat = value; OnPropertyChanged(); } }
        public decimal TotalGross { get => _totalGross; set { _totalGross = value; OnPropertyChanged(); } }
        public string InWords { get => _inWords; set { _inWords = value; OnPropertyChanged(); } }

        public RelayCommand AddItemCommand { get; }
        public RelayCommand RemoveItemCommand { get; }
        public RelayCommand SaveItemCommand { get; }
        public Func<InvoiceItemViewModel> NewItemCommand { get; }

        public InvoiceItemViewModel CreateItemViewModel()
        {
            var firstProduct = Products.FirstOrDefault();
            var firstRate = TaxRates.FirstOrDefault();
            var newItem = new InvoiceItem
            {
                InvoiceId = _currentInvoice()?.Id ?? 0,
                Quantity = 1,
                Product = firstProduct,
                ProductId = firstProduct?.Id ?? 0,
                TaxRate = firstRate,
                TaxRateId = firstRate?.Id ?? 0,
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            var vm = new InvoiceItemViewModel(newItem) { IsGross = _isGrossFunc() };
            vm.PropertyChanged += Item_PropertyChanged;
            return vm;
        }

        private void AddItem()
        {
            if (_currentInvoice() == null) return;
            var vm = CreateItemViewModel();
            Items.Add(vm);
            UpdateTotals();
            _raiseSaveChanged();
            _markDirty();
        }

        private void RemoveItem(InvoiceItemViewModel item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            Items.Remove(item);
            UpdateTotals();
            _raiseSaveChanged();
            _markDirty();
        }

        private async Task SaveItemAsync(InvoiceItemViewModel item)
        {
            var rate = TaxRates.FirstOrDefault(r => r.Percentage == item.TaxRatePercentage);
            if (rate == null)
            {
                var confirmAdd = DialogHelper.ShowConfirmation(
                    $"Nincs {item.TaxRatePercentage}% áfakulcs. Új áfakulcsot szeretnél létrehozni?",
                    "Megerősítés");
                if (!confirmAdd)
                {
                    rate = item.Item.Product?.TaxRate ?? rate;
                    item.TaxRatePercentage = rate?.Percentage ?? item.TaxRatePercentage;
                }
                else
                {
                    rate = new TaxRate
                    {
                        Name = $"ÁFA {item.TaxRatePercentage}%",
                        Percentage = item.TaxRatePercentage,
                        EffectiveFrom = DateTime.Today,
                        Active = true,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now
                    };
                    await _taxRateService.SaveAsync(rate);
                    TaxRates.Add(rate);
                }
            }

            if (rate != null)
            {
                item.Item.TaxRate = rate;
                item.Item.TaxRateId = rate.Id;
                item.TaxRate = rate;
            }

            if (item.Item.Product != null && rate != null && item.Item.Product.TaxRateId != rate.Id)
            {
                var confirm = DialogHelper.ShowConfirmation(
                    "Valóban módosítod az adott termék ÁFA-kulcsát?",
                    "Megerősítés");
                if (confirm)
                {
                    item.Item.Product.TaxRate = rate;
                    item.Item.Product.TaxRateId = rate.Id;
                    await _productService.SaveAsync(item.Item.Product);
                }
            }

            await _itemService.SaveAsync(item.Item);
            _statusService.Show($"Tétel mentve. ({DateTime.Now:g})");
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (InvoiceItemViewModel item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    item.IsGross = _isGrossFunc();
                }
            }
            if (e.OldItems != null)
            {
                foreach (InvoiceItemViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            UpdateTotals();
            _raiseSaveChanged();
            _markDirty();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateTotals();
            _raiseSaveChanged();
            _markDirty();
        }

        public void UpdateGrossMode(bool isGross)
        {
            foreach (var it in Items)
            {
                it.IsGross = isGross;
            }
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            var invoice = CurrentInvoice;
            if (invoice == null) return;

            var breakdown = _invoiceService.CalculateVatSummary(invoice)
                .Select(v => new VatBreakdownEntry
                {
                    Rate = v.Rate,
                    Net = v.Net,
                    Vat = v.Vat
                });

            VatBreakdown = new ObservableCollection<VatBreakdownEntry>(breakdown);
            TotalNet = VatBreakdown.Sum(v => v.Net);
            TotalVat = VatBreakdown.Sum(v => v.Vat);
            TotalGross = TotalNet + TotalVat;
            InWords = $"In Words: {NumberToWords((long)TotalGross)} Forint";
        }

        private static string NumberToWords(long number)
        {
            if (number == 0) return "zero";
            if (number < 0) return "minus " + NumberToWords(Math.Abs(number));
            string words = string.Empty;
            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }
            if (number > 0)
            {
                var unitsMap = new[] {"zero","one","two","three","four","five","six","seven","eight","nine","ten","eleven","twelve","thirteen","fourteen","fifteen","sixteen","seventeen","eighteen","nineteen"};
                var tensMap = new[] {"zero","ten","twenty","thirty","forty","fifty","sixty","seventy","eighty","ninety"};
                if (number < 20)
                {
                    words += unitsMap[number];
                }
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return words.Trim();
        }
    }
}
