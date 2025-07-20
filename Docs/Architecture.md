# Architecture Overview

This project is organized into five layers to support clean separation of concerns and easier maintenance.

## Domain
Contains the core entities, value objects, and business rules. These classes have no dependencies on other layers.

## Application
Coordinates use cases by exposing commands and queries. It defines repository interfaces, service contracts, DTOs, and validators.

## Infrastructure
Provides concrete implementations for data access and external integrations such as Entity Framework, file storage, or web services.

## Presentation
Hosts the WPF UI. ViewModels and Views reside here and interact with the Application layer to perform actions.

## Shared/Crosscutting
Holds logging, helper utilities, and any functionality reused across layers.
