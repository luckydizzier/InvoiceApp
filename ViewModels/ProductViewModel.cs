using System.Collections.ObjectModel;
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
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<TaxRate> _taxRates = new();
        private Product? _selectedProduct;

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

        public ProductViewModel(IProductService service, ITaxRateService taxRateService)
        {
            _service = service;
            _taxRateService = taxRateService;
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

            var rates = await _taxRateService.GetAllAsync();
            TaxRates = new ObservableCollection<TaxRate>(rates);
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
                await _service.SaveAsync(SelectedProduct);
            }
        }
    }
}
