using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;
using Serilog;

namespace InvoiceApp.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly IInvoiceService _service;
        private readonly IInvoiceItemService _itemService;
        private readonly IProductService _productService;
        private readonly ITaxRateService _taxRateService;
        private readonly ISupplierService _supplierService;
        private readonly IPaymentMethodService _paymentService;
        private readonly IChangeLogService _logService;
        private readonly SupplierViewModel _supplierViewModel;
        private ObservableCollection<Invoice> _invoices = new();
        private ObservableCollection<InvoiceItemViewModel> _items = new();
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<TaxRate> _taxRates = new();
        private ObservableCollection<Supplier> _suppliers = new();
        private ObservableCollection<PaymentMethod> _paymentMethods = new();
        private Invoice? _selectedInvoice;
        private string _statusMessage = string.Empty;
        private readonly System.Windows.Threading.DispatcherTimer _statusTimer;

        public IEnumerable<string> ValidationErrors => SelectedInvoice?.GetErrors(null).OfType<string>() ?? Enumerable.Empty<string>();

        public bool HasValidationErrors => ValidationErrors.Any();

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

        private void ShowStatus(string message)
        {
            StatusMessage = message;
            _statusTimer.Stop();
            _statusTimer.Start();
        }

        private void Invoice_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ValidationErrors));
            OnPropertyChanged(nameof(HasValidationErrors));
        }

        public Invoice? SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                if (_selectedInvoice != null)
                {
                    _selectedInvoice.ErrorsChanged -= Invoice_ErrorsChanged;
                }

                _selectedInvoice = value;

                if (_selectedInvoice != null)
                {
                    _selectedInvoice.ErrorsChanged += Invoice_ErrorsChanged;
                }
                Items = value != null
                    ? new ObservableCollection<InvoiceItemViewModel>(
                        value.Items.Select(i => new InvoiceItemViewModel(i)))
                    : new ObservableCollection<InvoiceItemViewModel>();
                SelectedSupplier = value?.Supplier;
                SelectedPaymentMethod = value?.PaymentMethod;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationErrors));
                OnPropertyChanged(nameof(HasValidationErrors));
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                ClearChanges();
            }
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
                    it.IsGross = IsGrossCalculation;
                }
                _items.CollectionChanged += Items_CollectionChanged;

                OnPropertyChanged();
                UpdateTotals();
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private InvoiceItemViewModel? _selectedItem;

        public InvoiceItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                // The DataGrid sometimes passes a {NewItemPlaceholder}; cast to
                // ensure only real view models are accepted.
                _selectedItem = value as InvoiceItemViewModel;
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

        public ObservableCollection<TaxRate> TaxRates
        {
            get => _taxRates;
            set { _taxRates = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set { _suppliers = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PaymentMethod> PaymentMethods
        {
            get => _paymentMethods;
            set { _paymentMethods = value; OnPropertyChanged(); }
        }

        private ObservableCollection<VatBreakdownEntry> _vatBreakdown = new();
        private decimal _totalNet;
        private decimal _totalVat;
        private decimal _totalGross;
        private string _inWords = string.Empty;
        private bool _isInvoiceListFocused = true;
        private bool _isRowDetailsVisible;
        private bool _hasChanges;

        /// <summary>
        /// Indicates whether there are unsaved modifications.
        /// </summary>
        public bool HasChanges
        {
            get => _hasChanges;
            private set { _hasChanges = value; OnPropertyChanged(); }
        }

        public ObservableCollection<VatBreakdownEntry> VatBreakdown
        {
            get => _vatBreakdown;
            set { _vatBreakdown = value; OnPropertyChanged(); }
        }

        private void MarkDirty()
        {
            HasChanges = true;
        }

        private void ClearChanges()
        {
            HasChanges = false;
        }

        public decimal TotalNet
        {
            get => _totalNet;
            set { _totalNet = value; OnPropertyChanged(); }
        }

        public decimal TotalVat
        {
            get => _totalVat;
            set { _totalVat = value; OnPropertyChanged(); }
        }

        public decimal TotalGross
        {
            get => _totalGross;
            set { _totalGross = value; OnPropertyChanged(); }
        }

        public string InWords
        {
            get => _inWords;
            set { _inWords = value; OnPropertyChanged(); }
        }

        public bool IsInvoiceListFocused
        {
            get => _isInvoiceListFocused;
            set { _isInvoiceListFocused = value; OnPropertyChanged(); }
        }

        public bool IsRowDetailsVisible
        {
            get => _isRowDetailsVisible;
            set { _isRowDetailsVisible = value; OnPropertyChanged(); }
        }

        public Supplier? SelectedSupplier
        {
            get => SelectedInvoice?.Supplier;
            set
            {
                if (SelectedInvoice != null && SelectedInvoice.Supplier != value)
                {
                    SelectedInvoice.Supplier = value;
                    SelectedInvoice.SupplierId = value?.Id ?? 0;
                    OnPropertyChanged();
                    _ = SuggestNextNumberAsync();
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                    MarkDirty();
                }
            }
        }

        public PaymentMethod? SelectedPaymentMethod
        {
            get => SelectedInvoice?.PaymentMethod;
            set
            {
                if (SelectedInvoice != null && SelectedInvoice.PaymentMethod != value)
                {
                    SelectedInvoice.PaymentMethod = value;
                    SelectedInvoice.PaymentMethodId = value?.Id ?? 0;
                    OnPropertyChanged();
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                    MarkDirty();
                }
            }
        }

        public bool IsGrossCalculation
        {
            get => SelectedInvoice?.IsGross ?? false;
            set
            {
                if (SelectedInvoice != null && SelectedInvoice.IsGross != value)
                {
                    SelectedInvoice.IsGross = value;
                    OnPropertyChanged();
                    foreach (var it in Items)
                    {
                        it.IsGross = value;
                    }
                    UpdateTotals();
                    MarkDirty();
                }
            }
        }

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand RemoveInvoiceCommand { get; }
        public ICommand SaveItemCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAndNewCommand { get; }
        public ICommand NewInvoiceCommand { get; }
        public ICommand AddSupplierCommand { get; }
        public Func<InvoiceItemViewModel> NewItemCommand { get; }

        public InvoiceViewModel(IInvoiceService service,
            IInvoiceItemService itemService,
            IProductService productService,
            ITaxRateService taxRateService,
            ISupplierService supplierService,
            IPaymentMethodService paymentService,
            IChangeLogService logService,
            SupplierViewModel supplierViewModel)
        {
            _service = service;
            _itemService = itemService;
            _productService = productService;
            _taxRateService = taxRateService;
            _supplierService = supplierService;
            _paymentService = paymentService;
            _logService = logService;
            _supplierViewModel = supplierViewModel;

            _statusTimer = new System.Windows.Threading.DispatcherTimer
            {
                // Extended duration so users can read status updates
                Interval = System.TimeSpan.FromSeconds(8)
            };
            _statusTimer.Tick += (s, e) => { StatusMessage = string.Empty; _statusTimer.Stop(); };

            AddItemCommand = new RelayCommand(_ => AddItem());
            RemoveItemCommand = new RelayCommand(obj =>
            {
                if (obj is InvoiceItemViewModel item)
                {
                    RemoveItem(item);
                }
            });
            RemoveInvoiceCommand = new RelayCommand(obj =>
            {
                if (obj is Invoice invoice)
                {
                    _service.DeleteAsync(invoice.Id).GetAwaiter().GetResult();
                    Invoices.Remove(invoice);
                }
            }, obj => obj is Invoice);
            SaveItemCommand = new RelayCommand(async obj =>
            {
                if (obj is InvoiceItemViewModel item)
                {
                    await SaveItemAsync(item);
                }
            }, obj => obj is InvoiceItemViewModel);
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => Validate());
            SaveAndNewCommand = new RelayCommand(async _ =>
            {
                await SaveAsync();
                await NewInvoice();
            }, _ => Validate());
            NewInvoiceCommand = new RelayCommand(async _ => await NewInvoice());
            AddSupplierCommand = new RelayCommand(_ => AddSupplier());
            NewItemCommand = CreateItemViewModel;

            if (SelectedInvoice != null)
            {
                SelectedInvoice.ErrorsChanged += Invoice_ErrorsChanged;
            }
        }

        public async Task LoadAsync()
        {
            Log.Debug("InvoiceViewModel.LoadAsync called");
            ShowStatus("Betöltés...");
            var items = await _service.GetAllAsync();
            Invoices = new ObservableCollection<Invoice>(items);

            var prods = await _productService.GetAllAsync();
            Products = new ObservableCollection<Product>(prods);

            var rates = await _taxRateService.GetAllAsync();
            TaxRates = new ObservableCollection<TaxRate>(rates);

            var sups = await _supplierService.GetAllAsync();
            Suppliers = new ObservableCollection<Supplier>(sups);

            var pays = await _paymentService.GetAllAsync();
            PaymentMethods = new ObservableCollection<PaymentMethod>(pays);

            var log = await _logService.GetLatestAsync();
            if (log != null)
            {
                ShowStatus($"Utolsó esemény: {log.Operation} ({log.DateCreated:g})");
            }
            else
            {
                ShowStatus(Invoices.Count == 0 ? "Üres lista." : $"{Invoices.Count} számla betöltve.");
            }
            ClearChanges();
        }

        public InvoiceItemViewModel CreateItemViewModel()
        {
            var firstProduct = Products.FirstOrDefault();
            var firstRate = TaxRates.FirstOrDefault();
            var newItem = new InvoiceItem
            {
                InvoiceId = SelectedInvoice?.Id ?? 0,
                Quantity = 1,
                Product = firstProduct,
                ProductId = firstProduct?.Id ?? 0,
                TaxRate = firstRate,
                TaxRateId = firstRate?.Id ?? 0,
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            var vm = new InvoiceItemViewModel(newItem);
            vm.PropertyChanged += Item_PropertyChanged;
            return vm;
        }

        private void AddItem()
        {
            Log.Debug("InvoiceViewModel.AddItem called");
            if (SelectedInvoice == null) return;
            var vm = CreateItemViewModel();
            vm.IsGross = IsGrossCalculation;
            Items.Add(vm);
            UpdateTotals();
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            MarkDirty();
        }

        private void RemoveItem(InvoiceItemViewModel item)
        {
            Log.Debug("InvoiceViewModel.RemoveItem called for {Product}", item.Item.Product?.Name);
            item.PropertyChanged -= Item_PropertyChanged;
            Items.Remove(item);
            UpdateTotals();
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            MarkDirty();
        }

        private void AddSupplier()
        {
            var supplier = _supplierViewModel.AddSupplier();
            Suppliers.Add(supplier);
            SelectedSupplier = supplier;
            ShowStatus("Új szállító hozzáadva");
        }

        public void EnsureSupplierExists(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var existing = Suppliers.FirstOrDefault(s =>
                string.Equals(s.Name, text, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                SelectedSupplier = existing;
                return;
            }

            var supplier = new Supplier
            {
                Name = text,
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            Suppliers.Add(supplier);
            SelectedSupplier = supplier;
            ShowStatus("Új szállító hozzáadva");
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
                    // revert to current product tax rate if user cancels
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

            if (item.Item.Product != null && rate != null &&
                item.Item.Product.TaxRateId != rate.Id)
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
            ShowStatus($"Tétel mentve. ({DateTime.Now:g})");
        }

        private async Task SuggestNextNumberAsync()
        {
            Log.Debug("InvoiceViewModel.SuggestNextNumberAsync called");
            if (SelectedInvoice == null || SelectedInvoice.Id != 0 || SelectedInvoice.SupplierId == 0)
            {
                return;
            }

            var latest = await _service.GetLatestForSupplierAsync(SelectedInvoice.SupplierId);
            if (latest != null)
            {
                SelectedInvoice.Number = IncrementNumber(latest.Number);
                OnPropertyChanged(nameof(SelectedInvoice));
                Log.Information("Suggested invoice number {Number}", SelectedInvoice.Number);
            }
        }

        private static string IncrementNumber(string lastNumber)
        {
            if (string.IsNullOrWhiteSpace(lastNumber)) return "1";

            var digits = new string(lastNumber.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
            if (digits.Length > 0 && int.TryParse(digits, out var n))
            {
                var prefix = lastNumber.Substring(0, lastNumber.Length - digits.Length);
                return prefix + (n + 1).ToString($"D{digits.Length}");
            }

            if (int.TryParse(lastNumber, out var value))
            {
                return (value + 1).ToString();
            }

            return lastNumber;
        }

        public async Task NewInvoice()
        {
            var invoice = new Invoice
            {
                Date = DateTime.Today,
                IsGross = false
            };

            var latest = await _service.GetLatestAsync();
            if (latest != null)
            {
                invoice.Supplier = latest.Supplier;
                invoice.SupplierId = latest.SupplierId;
                invoice.Number = IncrementNumber(latest.Number);
                invoice.PaymentMethod = latest.PaymentMethod;
                invoice.PaymentMethodId = latest.PaymentMethodId;
            }
            else
            {
                invoice.Supplier = Suppliers.FirstOrDefault();
                invoice.SupplierId = invoice.Supplier?.Id ?? 0;
                invoice.PaymentMethod = PaymentMethods.FirstOrDefault();
                invoice.PaymentMethodId = invoice.PaymentMethod?.Id ?? 0;
            }

            Invoices.Insert(0, invoice);
            SelectedInvoice = invoice;
            Items = new ObservableCollection<InvoiceItemViewModel>();
            ShowStatus("Új számla szerkesztése");
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            MarkDirty();
        }

        private bool Validate()
        {
            return SelectedInvoice?.IsValid() == true && Items.Count > 0;
        }

        private async Task SaveAsync()
        {
            Log.Debug("InvoiceViewModel.SaveAsync called");
            if (!Validate())
            {
                ShowStatus("Hibás adatok. Használd az Alt+1-6 billentyűket a mezők kiválasztásához.");
                return;
            }

            SelectedInvoice.Items = Items.Select(vm => vm.Item).ToList();
            if (SelectedSupplier != null)
            {
                await _supplierService.SaveAsync(SelectedSupplier);
            }
            await _service.SaveAsync(SelectedInvoice);

            foreach (var vm in Items)
            {
                var it = vm.Item;
                var rate = TaxRates.FirstOrDefault(r => r.Percentage == vm.TaxRatePercentage);
                if (rate == null)
                {
                    rate = new TaxRate
                    {
                        Name = $"ÁFA {vm.TaxRatePercentage}%",
                        Percentage = vm.TaxRatePercentage,
                        EffectiveFrom = DateTime.Today,
                        Active = true,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now
                    };
                    await _taxRateService.SaveAsync(rate);
                    TaxRates.Add(rate);
                }
                it.TaxRate = rate;
                it.TaxRateId = rate.Id;
                vm.TaxRate = rate;


                if (it.Product != null)
                {
                    it.Product.TaxRate = rate;
                    it.Product.TaxRateId = rate.Id;
                    await _productService.SaveAsync(it.Product);
                }
                await _itemService.SaveAsync(it);
            }

            ShowStatus($"Számla mentve. ({DateTime.Now:g})");
            Log.Information("Invoice {Id} saved", SelectedInvoice.Id);
            ClearChanges();
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (InvoiceItemViewModel item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    item.IsGross = IsGrossCalculation;
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
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            MarkDirty();
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateTotals();
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            MarkDirty();
        }

        private void UpdateTotals()
        {
            var breakdown = Items
                .GroupBy(i => i.TaxRatePercentage)
                .Select(g =>
                {
                    var net = g.Sum(x => x.Quantity * x.UnitPrice);
                    var vat = IsGrossCalculation ? net * g.Key / 100m : 0m;
                    return new VatBreakdownEntry
                    {
                        Rate = g.Key,
                        Net = net,
                        Vat = vat
                    };
                });

            VatBreakdown = new ObservableCollection<VatBreakdownEntry>(breakdown);
            TotalNet = VatBreakdown.Sum(v => v.Net);
            TotalVat = IsGrossCalculation ? VatBreakdown.Sum(v => v.Vat) : 0m;
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

        /// <summary>
        /// Selects the previous invoice in the list if available.
        /// </summary>
        public void SelectPreviousInvoice()
        {
            if (SelectedInvoice == null)
            {
                if (Invoices.Count > 0)
                    SelectedInvoice = Invoices[0];
                return;
            }

            var index = Invoices.IndexOf(SelectedInvoice);
            if (index > 0)
            {
                SelectedInvoice = Invoices[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új számlát szeretnél létrehozni?", "Megerősítés"))
                {
                    NewInvoiceCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Selects the next invoice in the list if available.
        /// </summary>
        public void SelectNextInvoice()
        {
            if (SelectedInvoice == null)
            {
                if (Invoices.Count > 0)
                    SelectedInvoice = Invoices[0];
                return;
            }

            var index = Invoices.IndexOf(SelectedInvoice);
            if (index < Invoices.Count - 1)
            {
                SelectedInvoice = Invoices[index + 1];
            }
        }

        /// <summary>
        /// Toggles visibility of invoice item row details.
        /// </summary>
        public void ToggleRowDetails()
        {
            IsRowDetailsVisible = !IsRowDetailsVisible;
        }

        /// <summary>
        /// Deletes the currently selected invoice.
        /// </summary>
        public void DeleteCurrentInvoice()
        {
            if (SelectedInvoice != null && RemoveInvoiceCommand.CanExecute(SelectedInvoice))
            {
                RemoveInvoiceCommand.Execute(SelectedInvoice);
            }
        }

        /// <summary>
        /// Saves the currently selected invoice item.
        /// </summary>
        public void SaveCurrentItem()
        {
            if (SelectedItem != null && SaveItemCommand.CanExecute(SelectedItem))
            {
                SaveItemCommand.Execute(SelectedItem);
            }
        }

        /// <summary>
        /// Removes the currently selected invoice item.
        /// </summary>
        public void DeleteCurrentItem()
        {
            if (SelectedItem != null && RemoveItemCommand.CanExecute(SelectedItem))
            {
                RemoveItemCommand.Execute(SelectedItem);
            }
        }

        /// <summary>
        /// Selects the previous invoice item if available.
        /// </summary>
        public void SelectPreviousItem()
        {
            if (SelectedItem == null)
            {
                if (Items.Count > 0)
                    SelectedItem = Items[0];
                return;
            }

            var index = Items.IndexOf(SelectedItem);
            if (index > 0)
            {
                SelectedItem = Items[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új tételt szeretnél létrehozni?", "Megerősítés"))
                {
                    AddItemCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Selects the next invoice item if available.
        /// </summary>
        public void SelectNextItem()
        {
            if (SelectedItem == null)
            {
                if (Items.Count > 0)
                    SelectedItem = Items[0];
                return;
            }

            var index = Items.IndexOf(SelectedItem);
            if (index < Items.Count - 1)
            {
                SelectedItem = Items[index + 1];
            }
        }

        /// <summary>
        /// Closes the currently visible row details panel.
        /// </summary>
        public void CancelItemEdit()
        {
            IsRowDetailsVisible = false;
            SelectedItem = null;
        }

        /// <summary>
        /// Saves the current invoice if possible.
        /// </summary>
        public void SaveCurrentInvoice()
        {
            if (SaveCommand.CanExecute(null))
            {
                SaveCommand.Execute(null);
            }
        }

        /// <summary>
        /// Cancels editing and focuses the invoice list.
        /// </summary>
        public void CancelHeaderOrSummary()
        {
            IsInvoiceListFocused = true;
        }
    }
}
