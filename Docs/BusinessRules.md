Business Rules
This document defines the core business rules enforced across the InvoiceApp system, beyond basic model validation. These rules ensure logical integrity, enforce domain-specific constraints, and protect against inconsistent states. All business logic must be enforced at the Services layer, not in the UI or repository.
📦 Entity Rules
Invoice
Invoices must reference an existing Supplier and PaymentMethod.
Each invoice must contain at least one InvoiceItem.
Invoice number must be unique per supplier.
Invoice date cannot be in the future.
Total amount must be calculated from items, not manually entered.
Finalized invoices cannot be deleted.
Draft invoices may be deleted but not after print/export.
InvoiceItem
Quantity must not be zero.
Negative quantity allowed only for returns/refunds.
Unit price must not be negative.
Each item must reference a valid Product and TaxRate.
Product
Cannot delete a product that is referenced by any invoice item.
Product name must be unique per supplier.
Products must have valid references to a Unit, TaxRate, and ProductGroup.
If a product has invoice history, its Unit, TaxRate, and Name cannot be changed.
Unit
Units cannot be deleted if any product references them.
Unit name must be unique (case-insensitive).
If Code is provided, it must be unique and non-empty.
TaxRate
Tax rates cannot be deleted if used in any product or invoice item.
EffectiveTo must be after EffectiveFrom, if provided.
Percentage must be between 0 and 100.
TaxRate is immutable once used.
ProductGroup
Cannot delete a product group if any product references it.
Group name must be unique.
Supplier
Suppliers cannot be deleted if they have invoices.
Tax ID must follow valid format, if provided.
Supplier name must be unique.
PaymentMethod
Cannot delete a payment method if referenced by any invoice.
DueInDays must be non-negative.
Name must be unique.
🔁 System-wide Rules
Soft Delete Enforcement: Any referenced entity must not be hard-deletable. Use Active = false instead.
Immutable Reference Rule: If an entity is used in financial records (invoices), its core identifying fields become immutable.
Cascading Disable: If a referenced entity becomes inactive, dependent entities must be warned or prevented from creation.
Calculation Authority: Services layer must calculate net/gross/VAT values – clients must never input precomputed totals.
Auto Numbering: Invoice numbers must be generated via service logic, based on supplier + sequence.
🧠 Enforcement Strategy
Validation logic is implemented in the Services layer, not in repositories or UI.
Violations must throw BusinessRuleViolationException or return structured Result<T> types.
Unit tests must cover all key rules using representative scenarios.
📚 Appendix: Recommended Service Interfaces
public interface IUnitService
   Task DeleteAsync(Guid id); // throws if used by products
public interface IProductService
{
   Task<Product> CreateAsync(Product p);
    Task UpdateAsync(Product p); // Enforce immutability if used
}
public interface IInvoiceService
{
    Task<Invoice> CreateAsync(Invoice i);
    Task UpdateAsync(Invoice i);
    Task DeleteAsync(Guid id); // Only if not finalized
    string GetNextInvoiceNumber(string supplierCode);
    decimal CalculateInvoiceTotal(Invoice invoice);
}
✅ Summary
This document defines all business constraints that must be checked outside of the database schema. These rules protect against invalid state transitions, guarantee data integrity, and ensure that domain logic is consistent regardless of UI or client application behavior.
All services must enforce these rules explicitly and predictably.
