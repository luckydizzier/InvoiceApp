# Pending Tasks for InvoiceApp MVP

This document outlines tasks to align the repository with the MVP requirements described in the project goals.

## Agent Tasks

```yaml
- id: task-F01-keyboard-navigation
  goal: Ensure invoice list supports keyboard navigation (Up/Down, Enter opens details).
  input: [Views/MainWindow.xaml.cs, Views/MainWindow.xaml]
  output: [Views/MainWindow.xaml.cs]
  depends_on: []
  constraints: []
  external: false

- id: task-F02-invoice-number-suggestion
  goal: Suggest next invoice number for a supplier when creating invoices.
  input: [ViewModels/InvoiceViewModel.cs, Services]
  output: [ViewModels/InvoiceViewModel.cs, Services]
  depends_on: []
  constraints: []
  external: false

- id: task-F03-search-filter
  goal: Add search and filter by supplier and date range on the invoice list.
  input: [ViewModels/InvoiceViewModel.cs, Views/MainWindow.xaml]
  output: [ViewModels/InvoiceViewModel.cs, Views/MainWindow.xaml]
  depends_on: []
  constraints: []
  external: false

- id: task-F05-net-gross-calculation
  goal: Implement net ↔ gross calculation per tax rate.
  input: [Models, Services, ViewModels]
  output: [Models, Services, ViewModels]
  depends_on: []
  constraints: []
  external: false

- id: task-F06-db-self-heal
  goal: Provide database self-healing with automatic creation, index check and backups.
  input: [StartupOrchestrator.cs, Data]
  output: [StartupOrchestrator.cs, Data]
  depends_on: []
  constraints: []
  external: false

- id: task-logging-json-rolling
  goal: Configure Serilog to use JSON logging with rolling files (5×5MB).
  input: [StartupOrchestrator.cs, appsettings.json]
  output: [StartupOrchestrator.cs, appsettings.json]
  depends_on: []
  constraints: []
  external: false
```

These tasks can be taken up by temporary agents according to the modular system. Each agent must keep to the listed inputs and outputs and perform validation such as diff checks or syntax verification.
