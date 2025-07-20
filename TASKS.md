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

- id: task-UI01-add-automation-names
  goal: Add x:Name or AutomationProperties.Name to key XAML controls.
  input: [Views]
  output: [Views]
  depends_on: []
  constraints: []
  external: false

- id: task-UI02-ui-test-harness
  goal: Create a small executable that opens specific dialogs for tests.
  input: [Views, InvoiceApp.csproj]
  output: [UITestHarness]
  depends_on: []
  constraints: []
  external: false

- id: task-UI03-appium-tests
  goal: Implement Appium/WebDriver tests.
  input: [UITestHarness, Views]
  output: [AppiumTests]
  depends_on: [task-UI02-ui-test-harness]
  constraints: []
  external: true

```

## Test Tasks

```yaml
- id: task-test-helpers
  goal: Add unit tests for helper utilities such as `AmountCalculator` and `DialogHelper`.
  input: [Helpers, InvoiceApp.Tests]
  output: [InvoiceApp.Tests]
  depends_on: []
  constraints: []
  external: false

- id: task-test-mappers
  goal: Add unit tests covering `EntityToDtoMapper` conversion methods.
  input: [Mappers, DTOs, Models, InvoiceApp.Tests]
  output: [InvoiceApp.Tests]
  depends_on: []
  constraints: []
  external: false

- id: task-test-validators
  goal: Add unit tests for DTO validators in `Validators`.
  input: [Validators, DTOs, InvoiceApp.Tests]
  output: [InvoiceApp.Tests]
  depends_on: []
  constraints: []
  external: false

- id: task-test-repositories
  goal: Add unit tests for repository logic using an in-memory database or mocks.
  input: [Repositories, Data, InvoiceApp.Tests]
  output: [InvoiceApp.Tests]
  depends_on: []
  constraints: []
  external: false

- id: task-test-services
  goal: Add unit tests for service layer methods, e.g. `InvoiceService.GetNextNumberAsync`.
  input: [Services, Repositories, DTOs, Models, InvoiceApp.Tests]
  output: [InvoiceApp.Tests]
  depends_on: [task-test-repositories]
  constraints: []
  external: false

- id: task-test-viewmodels
  goal: Expand view-model tests (e.g., `InvoiceViewModel`, `ItemsViewModel`) without touching XAML views.
  input: [ViewModels, Services, InvoiceApp.Tests]
  output: [InvoiceApp.Tests]
  depends_on: [task-test-services]
  constraints: []
  external: false

```

These tasks can be taken up by temporary agents according to the modular system. Each agent must keep to the listed inputs and outputs and perform validation such as diff checks or syntax verification.
