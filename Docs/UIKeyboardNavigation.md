# UI Keyboard Navigation

This document describes keyboard-only navigation patterns in InvoiceApp.
Users can interact with every view using only the keyboard.

## Basic Keys

| Key | Action |
|-----|-------|
| `Enter` | Accept input or advance to the next field. |
| `Escape` | Cancel the current action or return to the previous view. |
| `Ins` | Add a new item when focused on a list. |
| `Del` | Delete the currently selected item. |
| `Up/Down` | Move between list items or form fields. |
| `Left/Right` | Move the cursor within text fields. |

Press `Up` at the start of a list to quickly create a new entry.

## AppState Specific Navigation

### Dashboard (`DashboardView`)

- `Up`/`Down` navigate the menu items.
- `Enter` activates the highlighted menu option.
- `Escape` returns to the previous view or exits the application.


### Products (`ProductView`)

- `Up`/`Down` move between products.
- `Enter` edits the selected product.
- `Ins` adds a new product.
- `Del` deletes the selected product.
- `Escape` returns to the dashboard.
- `Up` before the first product triggers new product creation.

### ProductGroups (`ProductGroupView`)

- Same navigation as the Products view for managing groups.

### Suppliers (`SupplierView`)

- `Up`/`Down` navigate suppliers.
- `Enter` edits the highlighted supplier.
- `Ins` adds a supplier.
- `Del` removes the current supplier.
- `Escape` returns to the dashboard.

### TaxRates (`TaxRateView`)

- `Up`/`Down` move through tax rate entries.
- `Enter` edits the selected rate.
- `Ins` adds a new rate.
- `Del` deletes the selected rate.
- `Escape` returns to the dashboard.

### Units (`UnitView`)

- Navigation mirrors the TaxRates view.

### PaymentMethods (`PaymentMethodView`)

- `Up`/`Down` move between payment methods.
- `Enter` edits the active method.
- `Ins` adds a new method.
- `Del` removes the selected method.
- `Escape` returns to the dashboard.

### ConfirmDialog (`ConfirmDialog`)

- `Enter` accepts, `Escape` cancels.
