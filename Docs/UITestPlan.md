# UI Test Plan

This document outlines how to perform automated UI tests for **InvoiceApp**.

## Environment Requirements

- **WinAppDriver** – Install the latest stable release from Microsoft to enable Windows application automation.
- **Appium.WebDriver** – Reference the `Appium.WebDriver` NuGet package in the test project for Appium-based interaction.
- A Windows 10 or later workstation with Developer Mode enabled.

## Launching the Test Harness

UI tests run against a separate test harness executable. Launch `UITestHarness.exe` to host either the main window or individual dialogs under test. This allows WinAppDriver to attach without interfering with the production build.

## Example Test Cases

1. **Field value checks** – Verify that text boxes or combo boxes load with the expected default values.
2. **Button click simulations** – Invoke buttons programmatically and assert resulting state changes (e.g., dialog opens or data is saved).
3. **Navigation validation** – Use keyboard events to ensure focus moves correctly between controls and windows.

## Running Tests and Reporting

Tankó Ferenc runs the test suite locally using WinAppDriver with the harness executable. After execution, he records any failures and provides feedback in the issue tracker so the team can address defects.
The test sources reside in the `AppiumTests` project and rely on `UITestHarness.exe` as the target application.
