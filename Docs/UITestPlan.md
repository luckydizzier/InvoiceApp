# UI Test Plan

This document outlines how to perform automated UI tests for **InvoiceApp**.

## Environment Requirements

- **FlaUI** – Reference the `FlaUI.Core` and `FlaUI.UIA3` NuGet packages in the test project.
- A Windows 10 or later workstation.

## Launching the Test Harness

UI tests run against a separate test harness executable. Launch `UITestHarness.exe` to host either the main window or individual dialogs under test. FlaUI attaches directly to the process so no external driver is required.

## Example Test Cases

1. **Field value checks** – Verify that text boxes or combo boxes load with the expected default values.
2. **Button click simulations** – Invoke buttons programmatically and assert resulting state changes (e.g., dialog opens or data is saved).
3. **Navigation validation** – Use keyboard events to ensure focus moves correctly between controls and windows.

## Running Tests and Reporting

Tankó Ferenc runs the test suite locally with the harness executable. After execution, he records any failures and provides feedback in the issue tracker so the team can address defects.
The test sources reside in the `FlaUITests` project and rely on `UITestHarness.exe` as the target application.
