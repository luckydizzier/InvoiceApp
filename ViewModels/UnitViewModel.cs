using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class UnitViewModel : ViewModelBase
    {
        private readonly IUnitService _service;
        private ObservableCollection<Unit> _units = new();
        private Unit? _selectedUnit;

        public ObservableCollection<Unit> Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(); }
        }

        public Unit? SelectedUnit
        {
            get => _selectedUnit;
            set { _selectedUnit = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public UnitViewModel(IUnitService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddUnit());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is Unit unit)
                {
                    await DeleteUnitAsync(unit);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveSelectedAsync());
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Units = new ObservableCollection<Unit>(items);
        }

        private void AddUnit()
        {
            var unit = new Unit();
            Units.Add(unit);
            SelectedUnit = unit;
        }

        private async Task DeleteUnitAsync(Unit unit)
        {
            await _service.DeleteAsync(unit.Id);
            Units.Remove(unit);
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedUnit != null)
            {
                await _service.SaveAsync(SelectedUnit);
            }
        }
    }
}
