# Flipkart Automation

BDD-style UI test automation framework for [Flipkart](https://www.flipkart.com), built with **C#**, **Reqnroll**, **Playwright** and **NUnit**, using the **Page Object Model**. Tests run on Google Chrome and capture a screenshot after every step.

## Tech Stack

| Tool | Purpose |
|---|---|
| C# / .NET 8 | Language and runtime |
| Reqnroll | BDD framework (Gherkin feature files and step bindings) |
| Playwright for .NET | Browser automation (Chrome) |
| NUnit | Test runner and assertions |
| Microsoft.Extensions.Configuration | Reads settings from `appsettings.json` |

## Project Structure

```
FlipkartProj
├── Config            # appsettings.json, TestSettings (model), ConfigReader (loads settings)
├── Constants         # AppConstants: shared constant values
├── Drivers           # PlaywrightDriver: launches/closes Chrome, creates page and context
├── Entities          # Data models such as Product
├── Features          # Gherkin .feature files (test scenarios)
├── Hooks             # TestHooks: browser setup/teardown and per-step screenshots
├── Pages             # Page objects: HomePage, SearchResultPage
├── StepDefinitions   # Step bindings that connect Gherkin steps to page methods
└── Support           # Helpers such as ScreenshotHelper
```

## Modules

- **Config**: `appsettings.json` holds the base URL, browser, headless mode, slow-mo and timeout. `ConfigReader` loads it once into `TestSettings`, and values can be overridden by environment variables (useful in CI).
- **Drivers**: `PlaywrightDriver` starts Playwright and Chrome for each scenario, so scenarios stay isolated.
- **Pages**: Each page class holds its locators and actions. Steps never use locators directly.
- **StepDefinitions**: Thin step methods that call page objects and perform assertions.
- **Hooks**: `[BeforeScenario]` starts the browser, `[AfterStep]` saves a screenshot, and `[AfterScenario]` closes the browser.
- **Support**: Reusable utilities, such as saving screenshots into per-run, per-scenario folders.

## Prerequisites

- Visual Studio 2022 (with the .NET desktop development workload) or the .NET 8 SDK
- Google Chrome installed
- Reqnroll extension for Visual Studio (optional, for feature file support)

## Setup

```bash
git clone https://github.com/vinodksgithub/FlipkartAutomation.git
cd FlipkartAutomation
dotnet build
pwsh FlipkartProj/bin/Debug/net8.0/playwright.ps1 install
```

The last command downloads the Playwright browser drivers. It is only needed once.

## Running the Tests

**Visual Studio:** open **Test > Test Explorer**, then run all tests or a single scenario.

**Command line:**

```bash
# Run all tests
dotnet test

# Run only tests tagged @smoke
dotnet test --filter "Category=smoke"
```

## Configuration

Edit `Config/appsettings.json`:

```json
{
  "BaseUrl": "https://www.flipkart.com",
  "Browser": "chrome",
  "Headless": false,
  "SlowMo": 0,
  "DefaultTimeoutMs": 30000
}
```

To override a value without editing the file, for example in CI, set an environment variable before running:

```bash
set Headless=true
dotnet test
```

## Screenshots

A screenshot is saved after every step at:

```
FlipkartProj/bin/Debug/net8.0/Screenshots/<run-timestamp>/<scenario>/<step>.png
```

Failed steps are suffixed with `_FAILED`.

## Sample Scenario

```gherkin
@smoke
Scenario: Launch flipkart homepage and search
    Given User launches webpage
    And User closes any popup open in homepage
    Then User should see home page
    When User Inputs 'IPHONE 18' in search
    Then User should see search results
    And User should See first result as 'iphone 18'
```
