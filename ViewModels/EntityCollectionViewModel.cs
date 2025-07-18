using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace InvoiceApp.ViewModels
{
    public abstract class EntityCollectionViewModel<T> : ViewModelBase where T : class
    {
        private ObservableCollection<T> _items = new();
        private T? _selectedItem;
        private bool _hasChanges;
        private readonly bool _trackChanges;

        public ObservableCollection<T> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(); }
        }

        public T? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasChanges
        {
            get => _hasChanges;
            protected set
            {
                _hasChanges = value;
                OnPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        protected EntityCollectionViewModel(bool trackChanges = false)
        {
            _trackChanges = trackChanges;
            AddCommand = new RelayCommand(_ => AddItem());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is T item)
                {
                    var deleted = await DeleteItemAsync(item);
                    if (deleted)
                    {
                        Items.Remove(item);
                        AfterDelete(item);
                        if (_trackChanges)
                            MarkDirty();
                    }
                }
            }, _ => SelectedItem != null);
            SaveCommand = new RelayCommand(async _ =>
            {
                if (SelectedItem != null)
                {
                    await SaveItemAsync(SelectedItem);
                    AfterSave(SelectedItem);
                    if (_trackChanges)
                        ClearChanges();
                }
            }, _ => SelectedItem != null && (!_trackChanges || HasChanges) && CanSaveItem(SelectedItem));
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }

        protected virtual void AddItem()
        {
            var item = CreateNewItem();
            Items.Add(item);
            SelectedItem = item;
            if (_trackChanges)
                MarkDirty();
        }

        protected void MarkDirty() => HasChanges = true;
        protected void ClearChanges() => HasChanges = false;

        protected virtual bool CanSaveItem(T? item) => true;

        protected virtual T CreateNewItem() => Activator.CreateInstance<T>()!;
        protected virtual Task SaveItemAsync(T item) => Task.CompletedTask;
        protected virtual Task<bool> DeleteItemAsync(T item)
        {
            return Task.FromResult(true);
        }
        protected virtual void AfterDelete(T item) { }
        protected virtual void AfterSave(T item) { }
    }
}
