using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly IProductService _service;
        private ObservableCollection<Product> _products = new();
        private Product? _selectedProduct;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public ProductViewModel(IProductService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddProduct());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is Product product)
                {
                    await DeleteProductAsync(product);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveSelectedAsync());
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Products = new ObservableCollection<Product>(items);
        }

        private void AddProduct()
        {
            var product = new Product();
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
                await _service.SaveAsync(SelectedProduct);
            }
        }
    }
}
