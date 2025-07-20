using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace InvoiceApp.Application.DTOs
{
    public class InvoiceDisplayDto : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly InvoiceApp.Application.Validators.InvoiceDtoValidator _validator = new();
        private readonly Dictionary<string, List<string>> _errors = new();

        private int _id;
        private string _number = string.Empty;
        private string _issuer = string.Empty;
        private DateTime _date;
        private decimal _amount;
        private SupplierDto? _supplier;
        private PaymentMethodDto? _paymentMethod;
        private bool _isGross = true;
        private List<InvoiceItemDto> _items = new();

        public int Id { get => _id; set { if (_id != value) { _id = value; OnPropertyChanged(); } } }
        public string Number { get => _number; set { if (_number != value) { _number = value; OnPropertyChanged(); } } }
        public string Issuer { get => _issuer; set { if (_issuer != value) { _issuer = value; OnPropertyChanged(); } } }
        public DateTime Date { get => _date; set { if (_date != value) { _date = value; OnPropertyChanged(); } } }
        public decimal Amount { get => _amount; set { if (_amount != value) { _amount = value; OnPropertyChanged(); } } }

        public SupplierDto? Supplier
        {
            get => _supplier;
            set { if (_supplier != value) { _supplier = value; OnPropertyChanged(); } }
        }

        public PaymentMethodDto? PaymentMethod
        {
            get => _paymentMethod;
            set { if (_paymentMethod != value) { _paymentMethod = value; OnPropertyChanged(); } }
        }

        public bool IsGross { get => _isGross; set { if (_isGross != value) { _isGross = value; OnPropertyChanged(); } } }
        public List<InvoiceItemDto> Items { get => _items; set { if (_items != value) { _items = value; OnPropertyChanged(); } } }

        public int SupplierId { get => Supplier?.Id ?? 0; set { if (Supplier == null) Supplier = new SupplierDto { Id = value }; else if (Supplier.Id != value) { Supplier.Id = value; OnPropertyChanged(); } } }
        public int PaymentMethodId { get => PaymentMethod?.Id ?? 0; set { if (PaymentMethod == null) PaymentMethod = new PaymentMethodDto { Id = value }; else if (PaymentMethod.Id != value) { PaymentMethod.Id = value; OnPropertyChanged(); } } }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(kv => kv.Value);
            return _errors.TryGetValue(propertyName, out var list) ? list : Enumerable.Empty<string>();
        }

        private void Validate(string propertyName)
        {
            var dto = new InvoiceDto
            {
                Id = Id,
                Number = Number,
                Issuer = Issuer,
                Date = Date,
                Amount = Amount,
                SupplierId = SupplierId,
                PaymentMethodId = PaymentMethodId,
                IsGross = IsGross,
                Items = Items
            };
            var result = _validator.Validate(dto);
            var propErrors = result.Errors
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToList();

            if (propErrors.Any())
                _errors[propertyName] = propErrors;
            else
                _errors.Remove(propertyName);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName != null) Validate(propertyName);
        }
    }
}
