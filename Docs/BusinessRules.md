# Business Rules

This document defines the core business rules enforced across InvoiceApp. They
extend basic model validation to ensure logical integrity, enforce
domain‑specific constraints and protect against inconsistent states. All
business logic must be implemented in the **Services** layer, never in the UI or
repositories.

## Entity Rules

### Invoice

- Must reference an existing **Supplier** and **PaymentMethod**.
- Must contain at least one `InvoiceItem`.
- Invoice number must be unique per supplier.
- Date cannot be in the future.
- Total amount is calculated from items, not manually entered.
- Finalized invoices cannot be deleted.
- Draft invoices may be deleted but not after print or export.

### InvoiceItem

- Quantity must not be zero.
- Negative quantity allowed only for returns or refunds.
- Unit price must not be negative.
- Each item must reference a valid **Product** and **TaxRate**.

### Product

- Cannot delete a product referenced by any invoice item.
- Product name must be unique per supplier.
- Must reference a valid **Unit**, **TaxRate** and **ProductGroup**.
- If a product has invoice history, its Unit, TaxRate and Name cannot be changed.

### Unit

- Units cannot be deleted if any product references them.
- Unit name must be unique (case-insensitive).
- If a Code is provided, it must be unique and non-empty.

### TaxRate

- Tax rates cannot be deleted if used in any product or invoice item.
- `EffectiveTo` must be after `EffectiveFrom`, if provided.
- Percentage must be between 0 and 100.
- A TaxRate is immutable once used.

### ProductGroup

- Cannot delete a product group if any product references it.
- Group name must be unique.

### Supplier

- Suppliers cannot be deleted if they have invoices.
- Tax ID must follow a valid format, if provided.
- Supplier name must be unique.

### PaymentMethod

- Cannot delete a payment method if referenced by any invoice.
- `DueInDays` must be non-negative.
- Name must be unique.

## System-wide Rules

- **Soft Delete Enforcement:** referenced entities must not be hard deleted.
  Instead, set `Active = false`.
- **Immutable Reference Rule:** once an entity is used in financial records,
  its core identifying fields become immutable.
- **Cascading Disable:** if a referenced entity becomes inactive, dependent
  entities must be warned or prevented from creation.
- **Calculation Authority:** the Services layer calculates net/gross/VAT values;
  clients must never input precomputed totals.
- **Auto Numbering:** invoice numbers are generated via service logic based on
  supplier and sequence.

## Enforcement Strategy

Validation logic is implemented in the Services layer, not in repositories or
the UI. Violations must throw `BusinessRuleViolationException` or return a
structured `Result<T>` type. Unit tests should cover all key rules using
representative scenarios.

## Appendix: Recommended Service Interfaces

```csharp
public interface IUnitService
{
    Task DeleteAsync(Guid id); // throws if used by products
}

public interface IProductService
{
    Task<Product> CreateAsync(Product p);
    Task UpdateAsync(Product p); // enforce immutability if used
}

public interface IInvoiceService
{
    Task<Invoice> CreateAsync(Invoice i);
    Task UpdateAsync(Invoice i);
    Task DeleteAsync(Guid id); // only if not finalized
    string GetNextInvoiceNumber(string supplierCode);
    decimal CalculateInvoiceTotal(Invoice invoice);
}
```

## Summary

This document captures all business constraints that must be enforced outside of
the database schema. These rules protect against invalid state transitions,
guarantee data integrity and ensure consistent domain logic regardless of UI or
client application behavior. All services must enforce these rules explicitly and
predictably.
