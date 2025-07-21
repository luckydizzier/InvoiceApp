# EF Core Lazy Loading and Disposed Context

Entity Framework Core issues `LazyLoadOnDisposedContextWarning` when a proxy tries to load a navigation property after the `DbContext` that created it has been disposed. This typically occurs when entities retrieved within a `using` block are later used by the UI, and lazy-loading proxies attempt to resolve related data.

In the InvoiceApp, `Product` and `InvoiceItem` entities were loaded without their related objects (`Unit`, `ProductGroup`, `TaxRate`). The WPF views bind to these navigation properties, so the runtime attempted to lazy-load them once the `DbContext` was already disposed. This resulted in the runtime error:

```
Lazy-loading navigation 'Product.Unit' failed because the DbContext was disposed.
```

## Fix
The repositories now eagerly load the necessary navigation properties using `Include` and `ThenInclude`. This ensures each entity’s relationships are available before leaving the context scope, preventing lazy-loading from being triggered later in the UI.

If lazy loading is not required elsewhere, another approach would be disabling `UseLazyLoadingProxies()` or mapping entities to lightweight DTOs with the needed data. For now the eager-loading strategy resolves the issue.
