using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly IProductService _service;
        private readonly ITaxRateService _taxRateService;
        private readonly IUnitService _unitService;
        private readonly IProductGroupService _groupService;
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<TaxRate> _taxRates = new();
        private ObservableCollection<Unit> _units = new();
        private ObservableCollection<ProductGroup> _groups = new();
        private ICollectionView? _productsView;
        private Product? _selectedProduct;
        private string _searchText = string.Empty;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
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
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }

        public ProductViewModel(IProductService service,
                                ITaxRateService taxRateService,
                                IUnitService unitService,
                                IProductGroupService groupService)
        {
            _service = service;
            _taxRateService = taxRateService;
            _unitService = unitService;
            _groupService = groupService;
            AddCommand = new RelayCommand(_ => AddProduct());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is Product product && DialogHelper.ConfirmDeletion("terméket"))
                {
                    await DeleteProductAsync(product);
                    DialogHelper.ShowInfo("Törlés sikeres.");
                }
            }, _ => SelectedProduct != null);
            SaveCommand = new RelayCommand(async _ =>
            {
                await SaveSelectedAsync();
                DialogHelper.ShowInfo("Mentés kész.");
            }, _ => SelectedProduct != null && !string.IsNullOrWhiteSpace(SelectedProduct?.Name));
        }

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

        private void AddProduct()
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
            Products.Add(product);
            SelectedProduct = product;
        }

        private async Task DeleteProductAsync(Product product)
        {
            await _service.DeleteAsync(product.Id);
            Products.Remove(product);
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedProduct != null)
            {
                var percent = SelectedProduct.TaxRate?.Percentage ?? 0m;
                var rate = TaxRates.FirstOrDefault(r => r.Percentage == percent);
                if (rate == null)
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
                SelectedProduct.TaxRate = rate;
                SelectedProduct.TaxRateId = rate.Id;
                if (SelectedProduct.Unit != null)
                {
                    SelectedProduct.UnitId = SelectedProduct.Unit.Id;
                }
                if (SelectedProduct.ProductGroup != null)
                {
                    SelectedProduct.ProductGroupId = SelectedProduct.ProductGroup.Id;
                }
                SelectedProduct.Gross =
                    Math.Round(SelectedProduct.Net * (1 + (rate.Percentage / 100m)), 2);
                await _service.SaveAsync(SelectedProduct);
            }
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
