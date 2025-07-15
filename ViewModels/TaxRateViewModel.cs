using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class TaxRateViewModel : ViewModelBase
    {
        private readonly ITaxRateService _service;
        private ObservableCollection<TaxRate> _rates = new();
        private TaxRate? _selectedRate;

        public ObservableCollection<TaxRate> Rates
        {
            get => _rates;
            set { _rates = value; OnPropertyChanged(); }
        }

        public TaxRate? SelectedRate
        {
            get => _selectedRate;
            set
            {
                _selectedRate = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }

        public TaxRateViewModel(ITaxRateService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddRate());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is TaxRate rate && DialogHelper.ConfirmDeletion("áfakulcsot"))
                {
                    await DeleteRateAsync(rate);
                    DialogHelper.ShowInfo("Törlés sikeres.");
                }
            }, _ => SelectedRate != null);
            SaveCommand = new RelayCommand(async _ =>
            {
                await SaveSelectedAsync();
                DialogHelper.ShowInfo("Mentés kész.");
            }, _ => SelectedRate != null && !string.IsNullOrWhiteSpace(SelectedRate?.Name));
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Rates = new ObservableCollection<TaxRate>(items);
        }

        private void AddRate()
        {
            var rate = new TaxRate();
            Rates.Add(rate);
            SelectedRate = rate;
        }

        private async Task DeleteRateAsync(TaxRate rate)
        {
            await _service.DeleteAsync(rate.Id);
            Rates.Remove(rate);
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedRate != null)
            {
                await _service.SaveAsync(SelectedRate);
            }
        }
    }
}
