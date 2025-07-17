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
    public partial class Product : Base, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly ProductDtoValidator _validator = new();
        private readonly Dictionary<string, List<string>> _errors = new();

        private string _name = string.Empty;
        private decimal _net;
        private decimal _gross;
        private int _unitId;
        private Unit? _unit;
        private int _productGroupId;
        private ProductGroup? _productGroup;
        private int _taxRateId;
        private TaxRate? _taxRate;

        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        public decimal Net
        {
            get => _net;
            set { if (_net != value) { _net = value; OnPropertyChanged(); } }
        }

        public decimal Gross
        {
            get => _gross;
            set { if (_gross != value) { _gross = value; OnPropertyChanged(); } }
        }

        public int UnitId
        {
            get => _unitId;
            set { if (_unitId != value) { _unitId = value; OnPropertyChanged(); } }
        }

        public Unit? Unit
        {
            get => _unit;
            set { if (_unit != value) { _unit = value; OnPropertyChanged(); } }
        }

        public int ProductGroupId
        {
            get => _productGroupId;
            set { if (_productGroupId != value) { _productGroupId = value; OnPropertyChanged(); } }
        }

        public ProductGroup? ProductGroup
        {
            get => _productGroup;
            set { if (_productGroup != value) { _productGroup = value; OnPropertyChanged(); } }
        }

        public int TaxRateId
        {
            get => _taxRateId;
            set { if (_taxRateId != value) { _taxRateId = value; OnPropertyChanged(); } }
        }

        public TaxRate? TaxRate
        {
            get => _taxRate;
            set { if (_taxRate != value) { _taxRate = value; OnPropertyChanged(); } }
        }

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
    }
}
