using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

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
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }

        public ProductGroupViewModel(IProductGroupService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddGroup());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is ProductGroup group && DialogHelper.ConfirmDeletion("termékcsoportot"))
                {
                    await DeleteGroupAsync(group);
                    DialogHelper.ShowInfo("Törlés sikeres.");
                }
            }, _ => SelectedGroup != null);
            SaveCommand = new RelayCommand(async _ =>
            {
                await SaveSelectedAsync();
                DialogHelper.ShowInfo("Mentés kész.");
            }, _ => SelectedGroup != null && !string.IsNullOrWhiteSpace(SelectedGroup?.Name));
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
