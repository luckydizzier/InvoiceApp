# Business Rules

This document summarizes relationships and validation logic defined in the codebase. Source line numbers are approximate and may change over time.

## Key Relationships

- **Invoice** – `SupplierId`, `PaymentMethodId` and the `Items` list are defined around lines 52–82 of [Models/Invoice.cs](../Models/Invoice.cs).
- **InvoiceItem** – links to `Invoice`, `Product` and `TaxRate` at lines 11–18 of [Models/InvoiceItem.cs](../Models/InvoiceItem.cs).
- **Product** – references `Unit`, `ProductGroup` and `TaxRate` between lines 21–80 of [Models/Product.cs](../Models/Product.cs).
- **Base** – shared fields `Id`, `Active`, `DateCreated`, `DateUpdated` appear at lines 4–9 of [Models/Base.cs](../Models/Base.cs).

## Validation Rules

The validators use FluentValidation to enforce constraints:

| Entity | File & lines | Main Rules |
| ------ | ------------ | ---------- |
| Invoice | [InvoiceDtoValidator.cs](../Validators/InvoiceDtoValidator.cs) 10–21 | Number and Issuer required, Date not default, Amount >= 0, valid SupplierId, PaymentMethodId, at least one item |
| InvoiceItem | [InvoiceItemDtoValidator.cs](../Validators/InvoiceItemDtoValidator.cs) 10–13 | Quantity > 0, UnitPrice >= 0, ProductId > 0, TaxRateId > 0 |
| Product | [ProductDtoValidator.cs](../Validators/ProductDtoValidator.cs) 10–16 | Name required, Net & Gross >= 0, valid UnitId, ProductGroupId, TaxRateId |
| Supplier | [SupplierDtoValidator.cs](../Validators/SupplierDtoValidator.cs) 10–12 | Name required, Address & TaxId not empty when supplied |
| PaymentMethod | [PaymentMethodDtoValidator.cs](../Validators/PaymentMethodDtoValidator.cs) 10–11 | Name required, DueInDays >= 0 |
| TaxRate | [TaxRateDtoValidator.cs](../Validators/TaxRateDtoValidator.cs) 11–14 | Name required, Percentage >= 0, valid date range |
| Unit | [UnitDtoValidator.cs](../Validators/UnitDtoValidator.cs) 10–11 | Name required, Code required when provided |

## Implicit Logic

- Invoices must reference valid suppliers and payment methods and contain at least one item.
- Items always link to a product and a tax rate; products in turn depend on units, tax rates and groups.
- All entities record timestamps and an `Active` flag for soft deletion or state management.
- The [AmountCalculator](../Helpers/AmountCalculator.cs) helper (lines 8–24) computes net, VAT and gross amounts for items in both net and gross modes.
