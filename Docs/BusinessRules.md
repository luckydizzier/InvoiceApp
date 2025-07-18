# Business Rules

This document summarizes the key relationships and validation logic enforced by InvoiceApp's models and validators.

## Key Relationships

- **Invoice** references a `Supplier` and a `PaymentMethod` through the `SupplierId` and `PaymentMethodId` properties. Each invoice contains a list of `InvoiceItem` objects.
- **InvoiceItem** links back to its parent invoice and references a `Product` and a `TaxRate`.
- **Product** is associated with a `Unit`, a `ProductGroup`, and a `TaxRate`.
- **Base** is the common ancestor for all entities, providing `Id`, `Active`, `DateCreated`, and `DateUpdated` fields.

## Validation Rules

The validators enforce several business constraints:

- **Invoice** – `Number` and `Issuer` are required, `Date` cannot be the default value, `Amount` must be non‑negative, valid `SupplierId` and `PaymentMethodId` are required, and the invoice must contain at least one item.
- **InvoiceItem** – `Quantity` must be greater than zero, `UnitPrice` non‑negative, and both `ProductId` and `TaxRateId` must be positive.
- **Product** – `Name` is required, `Net` and `Gross` must be non‑negative, and valid `Unit`, `ProductGroup`, and `TaxRate` references are required.
- **Supplier** – `Name` is required; when provided, `Address` and `TaxId` must not be empty.
- **PaymentMethod** – `Name` is required and `DueInDays` cannot be negative.
- **TaxRate** – `Name` is required, `Percentage` must be non‑negative, `EffectiveFrom` cannot be default, and `EffectiveTo` must be later than `EffectiveFrom` when supplied.
- **Unit** – `Name` is required; if `Code` is provided it must not be empty.

## Implicit Logic

- Invoices must reference valid suppliers and payment methods and contain at least one item.
- Items always point to a product and a tax rate; products in turn require units, tax rates and product groups.
- All entities track creation and update timestamps along with an `Active` flag for soft deletion or state management.
- The `AmountCalculator` helper computes net, VAT and gross amounts for invoice items and supports both net and gross calculation modes.

