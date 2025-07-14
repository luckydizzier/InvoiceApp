# InvoiceApp

A small WPF application demonstrating an MRS (Model–Repository–Service) and MVVM architecture.

## Structure
- **Models** – POCO classes inheriting from `Base`.
- **Repositories** – data access abstractions (mock or EF Core).
- **Services** – business logic built on repositories.
- **ViewModels** – UI state and commands.
- **Views** – XAML-only UI files.

`StartupOrchestrator` configures dependency injection and Serilog logging. It also ensures a local SQLite database is present. Missing configuration or database files are created automatically. On first launch, users can choose to populate the database with sample data.
