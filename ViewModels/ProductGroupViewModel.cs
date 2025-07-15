using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class ProductGroupViewModel : ViewModelBase
    {
        private readonly IProductGroupService _service;
        private ObservableCollection<ProductGroup> _groups = new();
        private ProductGroup? _selectedGroup;

        public ObservableCollection<ProductGroup> Groups
        {
            get => _groups;
            set { _groups = value; OnPropertyChanged(); }
        }

        public ProductGroup? SelectedGroup
        {
            get => _selectedGroup;
            set { _selectedGroup = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public ProductGroupViewModel(IProductGroupService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddGroup());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is ProductGroup group)
                {
                    await DeleteGroupAsync(group);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveSelectedAsync());
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Groups = new ObservableCollection<ProductGroup>(items);
        }

        private void AddGroup()
        {
            var group = new ProductGroup();
            Groups.Add(group);
            SelectedGroup = group;
        }

        private async Task DeleteGroupAsync(ProductGroup group)
        {
            await _service.DeleteAsync(group.Id);
            Groups.Remove(group);
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedGroup != null)
            {
                await _service.SaveAsync(SelectedGroup);
            }
        }
    }
}
