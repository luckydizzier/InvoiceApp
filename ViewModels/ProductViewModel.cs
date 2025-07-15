using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

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

        public ProductViewModel(IProductService service)
        {
            _service = service;
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
