# InvoiceApp Architecture

This document outlines the high level organisation of the project. The solution is split into logical layers so that each concern can evolve independently.

## Domain
Pure C# classes describing entities and business rules. No framework or UI dependencies should be referenced here.

## Application
Orchestrates use cases. Contains interfaces for repositories and services along with DTOs and validators.

## Infrastructure
Concrete implementations of repositories or external integrations such as Entity Framework, file storage or web clients.

## Presentation
Hosts the WPF user interface. ViewModels execute use cases exposed by the Application layer.

## Shared / Crosscutting
Logging, helpers and other utilities that can be used across layers.
