using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class ProductViewModel : MasterDataViewModel<Product>, IHasChanges
    {
        public new RelayCommand SaveCommand { get; }
        private readonly IProductService _service;
        private readonly ITaxRateService _taxRateService;
        private readonly IUnitService _unitService;
        private readonly IProductGroupService _groupService;
        private readonly IStatusService _statusService;
        private ObservableCollection<TaxRate> _taxRates = new();
        private ObservableCollection<Unit> _units = new();
        private ObservableCollection<ProductGroup> _groups = new();
        private ICollectionView? _productsView;
        private string _searchText = string.Empty;

        public ObservableCollection<Product> Products
        {
            get => Items;
            set => Items = value;
        }

        public ICollectionView? ProductsView
        {
            get => _productsView;
            private set { _productsView = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TaxRate> TaxRates
        {
            get => _taxRates;
            set { _taxRates = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Unit> Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ProductGroup> Groups
        {
            get => _groups;
            set { _groups = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ProductsView?.Refresh();
            }
        }

        public Product? SelectedProduct
        {
            get => SelectedItem;
            set => SelectedItem = value;
        }


        public ProductViewModel(IProductService service,
                                ITaxRateService taxRateService,
                                IUnitService unitService,
                                IProductGroupService groupService,
                                IStatusService statusService)
            : base(true)
        {
            _service = service;
            _taxRateService = taxRateService;
            _unitService = unitService;
            _groupService = groupService;
            _statusService = statusService;
            ClearChanges();
            SaveCommand = new RelayCommand(async _ =>
            {
                if (SelectedProduct != null)
                {
                    await SaveItemAsync(SelectedProduct);
                    AfterSave(SelectedProduct);
                    ClearChanges();
                }
            }, _ => SelectedProduct != null && HasChanges && CanSaveItem(SelectedProduct));
        }

        public bool HasChanges => base.HasChanges;

        public void MarkDirty() => base.MarkDirty();

        public void ClearChanges() => base.ClearChanges();

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Products = new ObservableCollection<Product>(items);
            ProductsView = CollectionViewSource.GetDefaultView(Products);
            if (ProductsView != null)
            {
                ProductsView.Filter = obj =>
                {
                    if (obj is not Product p) return false;
                    return string.IsNullOrWhiteSpace(SearchText) ||
                           p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                };
            }

            var rates = await _taxRateService.GetAllAsync();
            TaxRates = new ObservableCollection<TaxRate>(rates);

            var units = await _unitService.GetAllAsync();
            Units = new ObservableCollection<Unit>(units);

            var groups = await _groupService.GetAllAsync();
            Groups = new ObservableCollection<ProductGroup>(groups);
        }

        protected override Product CreateNewItem()
        {
            var product = new Product();
            var firstRate = TaxRates.FirstOrDefault();
            if (firstRate != null)
            {
                product.TaxRate = firstRate;
                product.TaxRateId = firstRate.Id;
            }
            var firstUnit = Units.FirstOrDefault();
            if (firstUnit != null)
            {
                product.Unit = firstUnit;
                product.UnitId = firstUnit.Id;
            }
            var firstGroup = Groups.FirstOrDefault();
            if (firstGroup != null)
            {
                product.ProductGroup = firstGroup;
                product.ProductGroupId = firstGroup.Id;
            }
            return product;
        }

        protected override async Task<bool> DeleteItemAsync(Product product)
        {
            if (!DialogHelper.ConfirmDeletion("terméket"))
                return false;
            await _service.DeleteAsync(product.Id);
            _statusService.Show("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(Product product)
        {
            var percent = product.TaxRate?.Percentage ?? 0m;
            var rate = TaxRates.FirstOrDefault(r => r.Percentage == percent);
            if (rate == null)
            {
                var confirmAdd = DialogHelper.ShowConfirmation(
                    $"Nincs {percent}% áfakulcs. Új áfakulcsot szeretnél létrehozni?",
                    "Megerősítés");
                if (confirmAdd)
                {
                    rate = new TaxRate
                    {
                        Name = $"ÁFA {percent}%",
                        Percentage = percent,
                        EffectiveFrom = DateTime.Today,
                        Active = true,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now
                    };
                    await _taxRateService.SaveAsync(rate);
                    TaxRates.Add(rate);
                }
                else
                {
                    rate = TaxRates.FirstOrDefault(r => r.Id == product.TaxRateId);
                    if (rate != null)
                    {
                        product.TaxRate = rate;
                    }
                }
            }
            product.TaxRate = rate;
            product.TaxRateId = rate.Id;
            if (product.Unit != null)
            {
                product.UnitId = product.Unit.Id;
            }
            if (product.ProductGroup != null)
            {
                product.ProductGroupId = product.ProductGroup.Id;
            }
            product.Gross =
                Math.Round(product.Net * (1 + (rate.Percentage / 100m)), 2);
            await _service.SaveAsync(product);
            DialogHelper.ShowInfo("Mentés kész.");
        }

        public void SelectPreviousProduct()
        {
            if (SelectedProduct == null)
            {
                if (Products.Count > 0)
                    SelectedProduct = Products[0];
                return;
            }

            var index = Products.IndexOf(SelectedProduct);
            if (index > 0)
            {
                SelectedProduct = Products[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új terméket szeretnél létrehozni?", "Megerősítés"))
                {
                    AddCommand.Execute(null);
                }
            }
        }
    }
}
