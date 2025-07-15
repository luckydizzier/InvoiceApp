using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;

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
            set { _selectedRate = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public TaxRateViewModel(ITaxRateService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddRate());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is TaxRate rate)
                {
                    await DeleteRateAsync(rate);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveSelectedAsync());
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
