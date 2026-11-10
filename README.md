# Flipkart Automation

BDD-style test automation framework built with **C#**, **Reqnroll**, **Playwright** and **NUnit**. It covers two areas in one project:

- **UI tests** for [Flipkart](https://www.flipkart.com), using the Page Object Model on Google Chrome, with a screenshot after every step.
- **API tests** for [Restful-Booker](https://restful-booker.herokuapp.com), covering the full create, read, update, delete and confirm lifecycle, with no browser involved.

## Tech Stack

| Tool | Purpose |
|---|---|
| C# / .NET 9 | Language and runtime |
| Reqnroll | BDD framework (Gherkin feature files and step bindings) |
| Playwright for .NET | Browser automation (UI) and HTTP client (`APIRequestContext`, API) |
| NUnit | Test runner and assertions |
| Microsoft.Extensions.Configuration | Reads settings from `appsettings.json` |

## Project Structure

```
FlipkartFeature
├── Api               # ApiDriver (HTTP client setup), AuthClient (token), BookingClient (CRUD calls)
├── Config            # appsettings.json, TestSettings (model), ConfigReader (loads settings)
├── Constants         # AppConstants: shared constant values
├── Drivers           # PlaywrightDriver: launches/closes Chrome, creates page and context
├── Entities          # Models: Product (UI); Booking, BookingDates, AuthResponse, etc. (API)
├── Features          # Gherkin .feature files (Flipkart UI and Booking API)
├── Hooks             # TestHooks (browser + screenshots, UI only), ApiHooks (API client setup)
├── Pages             # Page objects: HomePage, SearchResultPage
├── StepDefinitions   # Step bindings for UI and API scenarios
└── Support           # BookingContext (shared scenario state), ScreenshotHelper
    └── Data          # BookingDataGenerator: random test data for API tests
```

## Modules

- **Config**: `appsettings.json` holds the UI base URL, browser, headless mode, slow-mo, timeout, and the API base URL and credentials. `ConfigReader` loads it once, and values can be overridden by environment variables.
- **Drivers / Pages**: `PlaywrightDriver` starts Chrome for each UI scenario. Page classes hold locators and actions, and steps never use locators directly.
- **Api**: `ApiDriver` creates one HTTP client per scenario. `AuthClient` fetches a token, and `BookingClient` wraps each endpoint and logs every request and response to the test output.
- **Entities / Support**: Typed models for requests and responses, `BookingContext` to share the booking ID and last response between steps, and `BookingDataGenerator` for dynamic test data.
- **Hooks**: Scenarios tagged `@api` skip the browser and screenshots. UI scenarios launch Chrome and save a screenshot after every step.

## Prerequisites

- Visual Studio 2022 (with the .NET desktop development workload) or the .NET 9 SDK
- Google Chrome (UI tests only)
- Internet access (both the Flipkart site and the Restful-Booker API are public)

## Setup

```bash
git clone https://github.com/vinodksgithub/FlipkartAutomation.git
cd FlipkartAutomation
dotnet build
pwsh FlipkartFeature/bin/Debug/net9.0/playwright.ps1 install
```

The last command downloads Playwright's browser drivers. It is only needed once.

## Running the Tests

**Visual Studio:** open **Test > Test Explorer**, then run all tests or a single scenario.

**Command line:**

```bash
# Run everything
dotnet test

# Run only the API tests
dotnet test --filter "Category=api"

# Run only the UI tests
dotnet test --filter "Category!=api"

# Run only tests tagged @smoke
dotnet test --filter "Category=smoke"
```

## API Tests

**API under test:** [Restful-Booker](https://restful-booker.herokuapp.com), a public practice API that stores the data it receives, so a created booking can be read back, updated and deleted.

All API scenarios are tagged `@api`, which means no browser is launched and no screenshots are taken.

### Scenarios

| Scenario | What it does |
|---|---|
| API server is reachable | `GET /ping` returns 201 |
| Booking CRUD lifecycle | Runs the full flow below against one booking |

### CRUD lifecycle

| Step | Request | Assertion |
|---|---|---|
| Create | `POST /booking` with randomly generated data | 200, a booking ID is returned, and the echoed data matches what was sent |
| Read and verify | `GET /booking/{id}` | 200, every field matches what was submitted, and the ID appears in a search by name |
| Update | `POST /auth` for a token, then `PATCH /booking/{id}` with a new last name | 200, the response shows the new last name, other fields are unchanged, and a follow-up `GET` confirms it was saved |
| Delete | `DELETE /booking/{id}` | 201 (Restful-Booker returns 201 Created for deletes) |
| Confirm delete | `GET /booking/{id}` | 404 Not Found |

### Notes

- **Dynamic test data:** names (with a random numeric suffix), price, dates and extras are generated on every run, so tests never depend on existing server data.
- **Authentication:** update and delete need a token, sent as the `Cookie: token=<value>` header. Credentials come from `appsettings.json`. These are the public demo credentials published by Restful-Booker.
- **Evidence:** each request and response is printed to the test output. In Test Explorer, select a test and choose **Open additional output for this result**.
- **Shared server:** the API is public and can be slow or reset its data. If a run fails with a timeout or a 5xx error, rerun it once.

## Configuration

Edit `Config/appsettings.json`:

```json
{
  "BaseUrl": "https://www.flipkart.com",
  "Browser": "chrome",
  "Headless": false,
  "SlowMo": 0,
  "DefaultTimeoutMs": 30000,
  "ApiBaseUrl": "https://restful-booker.herokuapp.com/",
  "ApiUsername": "admin",
  "ApiPassword": "password123"
}
```

To override a value without editing the file, for example in CI, set an environment variable before running:

```bash
set Headless=true
dotnet test
```

## Screenshots (UI tests)

A screenshot is saved after every UI step at:

```
FlipkartFeature/bin/Debug/net9.0/Screenshots/<run-timestamp>/<scenario>/<step>.png
```

Failed steps are suffixed with `_FAILED`. API tests do not produce screenshots.

## Sample Scenarios

**UI**

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

**API**

```gherkin
@api
Scenario: Booking CRUD lifecycle
    Given a booking is created with random data
    Then the create response should be successful
    When the booking is retrieved by its ID
    Then the booking details should match what was submitted
    When the booking last name is updated
    Then the response should show the updated last name
    When the booking is deleted
    Then the delete response should be 201
    When the deleted booking is retrieved
    Then the response should be 404 Not Found
```
