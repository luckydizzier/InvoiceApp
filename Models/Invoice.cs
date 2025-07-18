using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using InvoiceApp.Validators;

namespace InvoiceApp.Models
{
    public partial class Invoice : Base, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly InvoiceDtoValidator _validator = new();
        private readonly Dictionary<string, List<string>> _errors = new();

        private string _number = string.Empty;
        private string _issuer = string.Empty;
        private DateTime _date;
        private decimal _amount;
        private int _supplierId;
        private Supplier? _supplier;
        private int _paymentMethodId;
        private PaymentMethod? _paymentMethod;
        private bool _isGross = true;

        public string Number
        {
            get => _number;
            set { if (_number != value) { _number = value; OnPropertyChanged(); } }
        }

        public string Issuer
        {
            get => _issuer;
            set { if (_issuer != value) { _issuer = value; OnPropertyChanged(); } }
        }

        public DateTime Date
        {
            get => _date;
            set { if (_date != value) { _date = value; OnPropertyChanged(); } }
        }

        public decimal Amount
        {
            get => _amount;
            set { if (_amount != value) { _amount = value; OnPropertyChanged(); } }
        }

        public int SupplierId
        {
            get => _supplierId;
            set { if (_supplierId != value) { _supplierId = value; OnPropertyChanged(); } }
        }

        public Supplier? Supplier
        {
            get => _supplier;
            set { if (_supplier != value) { _supplier = value; OnPropertyChanged(); } }
        }

        public int PaymentMethodId
        {
            get => _paymentMethodId;
            set { if (_paymentMethodId != value) { _paymentMethodId = value; OnPropertyChanged(); } }
        }

        public PaymentMethod? PaymentMethod
        {
            get => _paymentMethod;
            set { if (_paymentMethod != value) { _paymentMethod = value; OnPropertyChanged(); } }
        }

        public bool IsGross
        {
            get => _isGross;
            set { if (_isGross != value) { _isGross = value; OnPropertyChanged(); } }
        }

        public List<InvoiceItem> Items { get; set; } = new();

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
            var result = _validator.Validate(this.ToDto());
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

        /// <summary>
        /// Validates the invoice using <see cref="InvoiceDtoValidator"/>.
        /// </summary>
        /// <returns>true if the invoice passes all validation rules; otherwise false.</returns>
        public bool IsValid()
        {
            return _validator.Validate(this.ToDto()).IsValid;
        }
    }
}
