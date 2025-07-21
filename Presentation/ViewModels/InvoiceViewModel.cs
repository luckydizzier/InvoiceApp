using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using InvoiceApp.Domain;
using InvoiceApp.Application.Services;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Mappers;
using InvoiceApp.Shared;
using InvoiceApp;
using InvoiceApp.Resources;
using Serilog;

namespace InvoiceApp.Presentation.ViewModels
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
        private readonly INavigationService _navigation;
        private readonly IStatusService _statusService;
        private ObservableCollection<InvoiceDisplayDto> _invoices = new();
        private InvoiceDisplayDto? _selectedInvoice;
        private Invoice? _selectedInvoiceEntity;
        private string _statusMessage = string.Empty;
        private readonly System.Windows.Threading.DispatcherTimer _statusTimer;
        public HeaderViewModel Header { get; } = null!;
        public ItemsViewModel ItemsView { get; } = null!;

        public IEnumerable<string> ValidationErrors => SelectedInvoice?.GetErrors(null).OfType<string>() ?? Enumerable.Empty<string>();

        public bool HasValidationErrors => ValidationErrors.Any();

        public ObservableCollection<InvoiceDisplayDto> Invoices
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
            _statusService.Show(message);
            StatusMessage = message;
            _statusTimer.Stop();
            _statusTimer.Start();
        }

        /// <summary>
        /// Updates the status message based on the active navigation state.
        /// </summary>
        public void UpdateNavigationStatus(AppState state)
        {
            string message = state switch
            {
                AppState.Header or AppState.ItemList or AppState.Summary
                    => "Nyomja meg az Esc-et a visszal\u00e9p\u00e9shez",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(message))
            {
                ShowStatus(message);
            }
            else
            {
                StatusMessage = string.Empty;
            }
        }

        private void Invoice_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ValidationErrors));
            OnPropertyChanged(nameof(HasValidationErrors));
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public InvoiceDisplayDto? SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                if (_selectedInvoice != null)
                {
                    _selectedInvoice.ErrorsChanged -= Invoice_ErrorsChanged;
                }

                _ = SetSelectedInvoiceAsync(value);
                ClearChanges();
            }
        }

        private async Task SetSelectedInvoiceAsync(InvoiceDisplayDto? value)
        {
            Invoice? invoice = value?.ToEntity();

            if (invoice != null)
            {
                try
                {
                    var detailed = await _service.GetDetailsAsync(invoice.Id);
                    if (detailed != null)
                    {
                        invoice = detailed;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to load invoice details for {Id}", invoice.Id);
                }
            }

            _selectedInvoiceEntity = invoice;
            _selectedInvoice = invoice?.ToDisplayDto();

            if (_selectedInvoice != null)
            {
                _selectedInvoice.ErrorsChanged += Invoice_ErrorsChanged;
            }

            Header.SelectedInvoice = _selectedInvoiceEntity;
            Items = _selectedInvoiceEntity != null
                ? new ObservableCollection<InvoiceItemViewModel>(
                    _selectedInvoiceEntity.Items.Select(i => new InvoiceItemViewModel(i)))
                : new ObservableCollection<InvoiceItemViewModel>();
            SelectedSupplier = _selectedInvoiceEntity?.Supplier;
            SelectedPaymentMethod = _selectedInvoiceEntity?.PaymentMethod;
            OnPropertyChanged(nameof(SelectedInvoice));
            OnPropertyChanged(nameof(ValidationErrors));
            OnPropertyChanged(nameof(HasValidationErrors));
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public ObservableCollection<InvoiceItemViewModel> Items
        {
            get => ItemsView.Items;
            set => ItemsView.Items = value;
        }

        public InvoiceItemViewModel? SelectedItem
        {
            get => ItemsView.SelectedItem;
            set => ItemsView.SelectedItem = value;
        }

        public ObservableCollection<Product> Products
        {
            get => ItemsView.Products;
            set => ItemsView.Products = value;
        }

        public ObservableCollection<TaxRate> TaxRates
        {
            get => ItemsView.TaxRates;
            set => ItemsView.TaxRates = value;
        }

        public ObservableCollection<Supplier> Suppliers
        {
            get => Header.Suppliers;
            set => Header.Suppliers = value;
        }

        public ObservableCollection<PaymentMethod> PaymentMethods
        {
            get => Header.PaymentMethods;
            set => Header.PaymentMethods = value;
        }

        private bool _isInvoiceListFocused = true;
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
            get => ItemsView.VatBreakdown;
            set => ItemsView.VatBreakdown = value;
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
            get => ItemsView.TotalNet;
            set => ItemsView.TotalNet = value;
        }

        public decimal TotalVat
        {
            get => ItemsView.TotalVat;
            set => ItemsView.TotalVat = value;
        }

        public decimal TotalGross
        {
            get => ItemsView.TotalGross;
            set => ItemsView.TotalGross = value;
        }

        public string InWords
        {
            get => ItemsView.InWords;
            set => ItemsView.InWords = value;
        }

        public bool IsInvoiceListFocused
        {
            get => _isInvoiceListFocused;
            set { _isInvoiceListFocused = value; OnPropertyChanged(); }
        }

        public bool IsRowDetailsVisible
        {
            get => ItemsView.IsRowDetailsVisible;
            set => ItemsView.IsRowDetailsVisible = value;
        }

        public Supplier? SelectedSupplier
        {
            get => Header.SelectedSupplier;
            set => Header.SelectedSupplier = value;
        }

        public PaymentMethod? SelectedPaymentMethod
        {
            get => Header.SelectedPaymentMethod;
            set => Header.SelectedPaymentMethod = value;
        }

        public bool IsGrossCalculation
        {
            get => Header.IsGrossCalculation;
            set => Header.IsGrossCalculation = value;
        }

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand RemoveInvoiceCommand { get; }
        public ICommand SaveItemCommand { get; }
        public ICommand SaveCommand { get; } = null!;
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
            SupplierViewModel supplierViewModel,
            INavigationService navigation,
            IStatusService statusService)
        {
            _service = service;
            _itemService = itemService;
            _productService = productService;
            _taxRateService = taxRateService;
            _supplierService = supplierService;
            _paymentService = paymentService;
            _logService = logService;
            _supplierViewModel = supplierViewModel;
            _navigation = navigation;
            _statusService = statusService;

            Header = new HeaderViewModel(
                _supplierViewModel,
                _statusService.Show,
                SuggestNextNumberAsync,
                () => ((RelayCommand)SaveCommand!).RaiseCanExecuteChanged(),
                MarkDirty,
                isGross => ItemsView!.UpdateGrossMode(isGross));

            ItemsView = new ItemsViewModel(
                _itemService,
                _productService,
                _taxRateService,
                _service,
                _statusService,
                () => ((RelayCommand)SaveCommand!).RaiseCanExecuteChanged(),
                MarkDirty,
                () => Header.IsGrossCalculation,
                () => _selectedInvoiceEntity);

            _statusTimer = new System.Windows.Threading.DispatcherTimer
            {
                // Extended duration so users can read status updates
                Interval = System.TimeSpan.FromSeconds(8)
            };
            _statusTimer.Tick += (s, e) => { StatusMessage = string.Empty; _statusTimer.Stop(); };

            AddItemCommand = ItemsView.AddItemCommand;
            RemoveItemCommand = ItemsView.RemoveItemCommand;
            RemoveInvoiceCommand = new AsyncRelayCommand(async obj =>
            {
                if (obj is InvoiceDisplayDto dto &&
                    DialogHelper.ConfirmDeletion("számlát"))
                {
                    await _service.DeleteAsync(dto.Id);
                    Invoices.Remove(dto);
                    ShowStatus("Számla törölve.");
                }
            }, obj => obj is InvoiceDisplayDto);
            SaveItemCommand = ItemsView.SaveItemCommand;
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => Validate());
            SaveAndNewCommand = new RelayCommand(async _ =>
            {
                await SaveAsync();
                await NewInvoice();
            }, _ => Validate());
            NewInvoiceCommand = new RelayCommand(async _ => await NewInvoice());
            AddSupplierCommand = Header.AddSupplierCommand;
            NewItemCommand = ItemsView.NewItemCommand;

            if (SelectedInvoice != null)
            {
                SelectedInvoice.ErrorsChanged += Invoice_ErrorsChanged;
            }
        }

        public async Task LoadAsync()
        {
            Log.Debug("InvoiceViewModel.LoadAsync called");
            try
            {
                IsLoading = true;
                ShowStatus("Betöltés...");
                var items = await _service.GetHeadersAsync();
                Invoices = new ObservableCollection<InvoiceDisplayDto>(items.Select(i => i.ToDisplayDto()));

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
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load invoices");
                DialogHelper.ShowError(Resources.Strings.InvoiceLoadError);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public InvoiceItemViewModel CreateItemViewModel()
        {
            var firstProduct = Products.FirstOrDefault();
            var firstRate = TaxRates.FirstOrDefault();
            var newItem = new InvoiceItem
            {
                InvoiceId = _selectedInvoiceEntity?.Id ?? 0,
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
            ItemsView.AddItemCommand.Execute(null);
        }

        private void RemoveItem(InvoiceItemViewModel item)
        {
            ItemsView.RemoveItemCommand.Execute(item);
        }

        private void AddSupplier()
        {
            Header.AddSupplierCommand.Execute(null);
        }

        public void EnsureSupplierExists(string text)
        {
            Header.EnsureSupplierExists(text);
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

            var next = await _service.GetNextNumberAsync(SelectedInvoice.SupplierId);
            if (!string.IsNullOrEmpty(next))
            {
                SelectedInvoice.Number = next;
                OnPropertyChanged(nameof(SelectedInvoice));
                Log.Information("Suggested invoice number {Number}", SelectedInvoice.Number);
            }
        }


        public async Task NewInvoice()
        {
            var invoice = new InvoiceDisplayDto
            {
                Date = DateTime.Today,
                IsGross = false
            };

            var latest = await _service.GetLatestAsync();
            if (latest != null)
            {
                invoice.Supplier = latest.Supplier?.ToDto();
                invoice.PaymentMethod = latest.PaymentMethod?.ToDto();
                invoice.Number = await _service.GetNextNumberAsync(invoice.SupplierId);
            }
            else
            {
                var s = Suppliers.FirstOrDefault();
                if (s != null) invoice.Supplier = s.ToDto();
                var p = PaymentMethods.FirstOrDefault();
                if (p != null) invoice.PaymentMethod = p.ToDto();
            }

            Invoices.Insert(0, invoice);
            SelectedInvoice = invoice;
            IsInvoiceListFocused = false;
            _navigation.PushSubstate(AppState.Header);
            Items = new ObservableCollection<InvoiceItemViewModel>();
            ShowStatus("Új számla szerkesztése");
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            MarkDirty();
        }

        private bool Validate()
        {
            return SelectedInvoice != null
                && !SelectedInvoice.HasErrors;
        }

        private async Task SaveAsync()
        {
            Log.Debug("InvoiceViewModel.SaveAsync called");
            if (!Validate())
            {
                ShowStatus("Hibás adatok. Használd az Alt+1-6 billentyűket a mezők kiválasztásához.");
                return;
            }

            SelectedInvoice!.Items = Items.Select(vm => vm.Item.ToDto()).ToList();

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
                    TaxRates.Add(rate);
                }
                it.TaxRate = rate;
                it.TaxRateId = rate.Id;
                vm.TaxRate = rate;


                if (it.Product != null)
                {
                    it.Product.TaxRate = rate;
                    it.Product.TaxRateId = rate.Id;
                }
            }

            _selectedInvoiceEntity = SelectedInvoice?.ToEntity();
            if (_selectedInvoiceEntity != null)
            {
                await _service.SaveInvoiceWithItemsAsync(_selectedInvoiceEntity, Items.Select(i => i.Item));
                SelectedInvoice = _selectedInvoiceEntity.ToDisplayDto();
            }

            ShowStatus($"Számla mentve. ({DateTime.Now:g})");
            Log.Information("Invoice {Id} saved", SelectedInvoice!.Id);
            ClearChanges();
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ItemsView.UpdateGrossMode(Header.IsGrossCalculation);
        }

        private void UpdateTotals()
        {
            ItemsView.UpdateGrossMode(Header.IsGrossCalculation);
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
            if (_navigation.CurrentState != AppState.MainWindow)
                return;

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
