using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class PaymentMethodViewModel : ViewModelBase
    {
        private readonly IPaymentMethodService _service;
        private ObservableCollection<PaymentMethod> _methods = new();
        private PaymentMethod? _selectedMethod;

        public ObservableCollection<PaymentMethod> Methods
        {
            get => _methods;
            set { _methods = value; OnPropertyChanged(); }
        }

        public PaymentMethod? SelectedMethod
        {
            get => _selectedMethod;
            set { _selectedMethod = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public PaymentMethodViewModel(IPaymentMethodService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddMethod());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is PaymentMethod method)
                {
                    await DeleteMethodAsync(method);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveSelectedAsync());
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Methods = new ObservableCollection<PaymentMethod>(items);
        }

        private void AddMethod()
        {
            var method = new PaymentMethod();
            Methods.Add(method);
            SelectedMethod = method;
        }

        private async Task DeleteMethodAsync(PaymentMethod method)
        {
            await _service.DeleteAsync(method.Id);
            Methods.Remove(method);
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedMethod != null)
            {
                await _service.SaveAsync(SelectedMethod);
            }
        }
    }
}
