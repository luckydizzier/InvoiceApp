# AppState and Navigation

This document lists the official `AppState` values used by InvoiceApp and the default keyboard navigation events.

## AppState values

| State | Description |
|-------|-------------|
| `MainWindow` | Root container window for the application. |
| `Dashboard` | Overview screen showing recent activity. |
| `InvoiceList` | Displays all invoices. |
| `InvoiceEditor` | (unused) Former container for invoice editing. |
| `Header` | Editor substate for header information. |
| `ItemList` | Editor substate for editing invoice items. |
| `Summary` | Editor substate summarising totals. |
| `Products` | Manage product catalogue. |
| `ProductGroups` | Manage groups of products. |
| `Suppliers` | Manage suppliers. |
| `TaxRates` | Manage VAT or tax rates. |
| `Units` | Manage units of measure. |
| `PaymentMethods` | Manage payment methods. |
| `ConfirmDialog` | Generic confirmation dialog. |

## Keyboard navigation

Global keyboard shortcuts are mapped as follows:

| Key | Action |
|-----|-------|
| `F1` | Open Dashboard |
| `F2` | Open Invoice list |
| `F3` | Start Invoice editor |
| `F4` | Open Products view |
| `F5` | Open Product groups |
| `F6` | Open Suppliers |
| `F7` | Open Tax rates |
| `F8` | Open Payment methods |
| `Up/Down` | Navigate lists |
| `Enter` | Activate or accept |
| `Escape` | Go back |

The invoice editor substates (`Header`, `ItemList`, `Summary`) respond only to **Enter**, **Escape** and the **arrow** keys for navigation.
