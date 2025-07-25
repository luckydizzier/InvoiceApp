# InvoiceApp Architecture

This document outlines how the project is organised into clear layers so each concern can evolve independently.

## Domain
Pure C# classes describing entities and business rules. No framework or UI dependencies belong here.

## Application
Orchestrates use cases and contains repository/service interfaces, DTOs, mappers and validators. This layer coordinates domain entities without knowing the UI or infrastructure details.

## Infrastructure
Concrete implementations of repositories and other integrations such as Entity Framework, file storage or web clients. It plugs into interfaces defined in the Application layer.

## Presentation
Hosts the WPF user interface. ViewModels call use case classes from the Application layer. Views remain XAML only.

## Shared / Crosscutting
Logging, helpers and other utilities that are reused across layers.

## Typical Workflow
1. A ViewModel in the Presentation layer invokes a use case from the Application layer.
2. The use case validates incoming data and interacts with repository interfaces.
3. Infrastructure classes implement those interfaces and persist or retrieve Domain entities.
4. Results are returned back through the Application layer to the UI.

This layered approach keeps dependencies flowing inward: Presentation → Application → Domain while Infrastructure implements Application contracts.

### Workflow Examples

**Creating an Invoice**
1. `InvoiceViewModel` collects user input and calls the `CreateInvoice` use case.
2. `CreateInvoice` validates the `Invoice` entity via `InvoiceService`.
3. `InvoiceService` persists the entity through `IInvoiceRepository`.
4. `EfInvoiceRepository` saves to SQLite via `InvoiceContext`.
5. The ViewModel refreshes its view using the returned DTO.

**Listing Products**
1. `DashboardViewModel` requests product data via the `ListProducts` use case.
2. `ListProducts` retrieves `Product` entities from `IProductRepository`.
3. The data is mapped to DTOs and presented in the UI.

