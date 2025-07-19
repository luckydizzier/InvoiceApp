# User Goals

This document summarises the user-facing features and planned enhancements for
**InvoiceApp**.

## Manage Invoices

- View, edit, create and delete invoices.
- Keyboard shortcuts open the editor and add or delete invoices.
  *Source: App state list and navigation keys*

## Manage Products, Suppliers and Related Data

- Maintain the product catalogue, product groups, suppliers, tax rates, units
  and payment methods.
- Each management view supports keyboard navigation for adding, editing or
  removing entries.
  *Source: App state definitions and keyboard navigation guidelines*

## Keyboard Navigation Across the Application

- The entire UI is operable using only the keyboard.
- Global shortcuts include **F1** for the dashboard and **F2** for the invoice
  list.
  *Source: Keyboard navigation table*

## Automatic Database Setup with Sample Data

- On first launch the application creates a SQLite database.
- Users may optionally populate it with sample data.
- Sample data generation supports custom quantity ranges for invoice items.
  *Source: Startup description and feature list*

## Logging via Serilog

- Logs are written to JSON files with daily rolling by default.
- A planned task configures five rolling files of 5&nbsp;MB each.
  *Source: Feature list and planned task for JSON logging*

## Search and Filter Invoices

- Planned ability to filter the invoice list by supplier and date range.
  *Source: Task definition*

## Next Invoice Number Suggestion

- When creating a new invoice the next number is automatically proposed for the
  selected supplier.
  *Source: Task definition*

## Net ↔ Gross Calculation by Tax Rate

- Convert between net and gross amounts based on the selected tax rate.
  *Source: Task definition*

## Database Self‑Healing

- Automatic creation, index checks and backups keep the database operational.
  *Source: Task definition*

## Data Export and Reporting (Planned)

- Future features will export invoices (CSV/PDF) and analyse data.
  *Source: Future enhancements list*

## Localization and Dark Mode (Planned)

- Planned support for multiple languages and a dark UI theme.
  *Source: Future enhancements list*

## Ongoing Improvements

- UI refinements, performance optimization, expanded unit tests and better
  documentation.
- Success messages appear in the status bar instead of modal dialogs.
  *Source: Future enhancements list and README*

These goals outline the intended features and enhancements for InvoiceApp as
documented in the repository.
