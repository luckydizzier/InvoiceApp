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

When pressing `Up` while the first item in a list is selected, the application
asks whether to create a new entry. Confirming will add a blank item to the
list.

## AppState Specific Navigation

### Dashboard (`DashboardView`)

- `Up`/`Down` navigate the menu items.
- `Enter` activates the highlighted menu option.
- `Escape` returns to the previous view or exits the application.

### InvoiceList (`InvoiceListView`)

- `Up`/`Down` move between invoices.
- `Enter` opens the selected invoice in the editor.
- `Ins` creates a new invoice.
- `Del` deletes the highlighted invoice.
- `Escape` returns to the dashboard.
- Pressing `Up` before the first entry prompts to create a new invoice.

### InvoiceEditor (`InvoiceEditorView`)

- Consists of `InvoiceHeaderView`, `InvoiceItemDataGrid` and
  `InvoiceSummaryPanel`.
- `Enter` advances to the next field; on the last field it moves to the next
  editor step.
- `Escape` cancels editing and returns to the invoice list.
- `Ins` adds a new item while the item grid is focused.
- `Del` removes the selected item from the grid.
- `Up`/`Down` navigate rows in the item grid; `Left`/`Right` move within cells.
- Pressing `Up` on the first row prompts to create a new item.

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
