using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

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
            set
            {
                _selectedMethod = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }

        public PaymentMethodViewModel(IPaymentMethodService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddMethod());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is PaymentMethod method && DialogHelper.ConfirmDeletion("fizetési módot"))
                {
                    await DeleteMethodAsync(method);
                    DialogHelper.ShowInfo("Törlés sikeres.");
                }
            }, _ => SelectedMethod != null);
            SaveCommand = new RelayCommand(async _ =>
            {
                await SaveSelectedAsync();
                DialogHelper.ShowInfo("Mentés kész.");
            }, _ => SelectedMethod != null && !string.IsNullOrWhiteSpace(SelectedMethod?.Name));
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

        public void SelectPreviousMethod()
        {
            if (SelectedMethod == null)
            {
                if (Methods.Count > 0)
                    SelectedMethod = Methods[0];
                return;
            }

            var index = Methods.IndexOf(SelectedMethod);
            if (index > 0)
            {
                SelectedMethod = Methods[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új fizetési módot szeretnél létrehozni?", "Megerősítés"))
                {
                    AddCommand.Execute(null);
                }
            }
        }
    }
}
