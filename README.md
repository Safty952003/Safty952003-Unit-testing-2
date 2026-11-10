# Mini Checkout Engine

## Student Information
- **Name:** [Mahmoud Mohammed Elsafty]
- **Group / ID:** [G1]

## Project Overview
Mini Checkout Engine is a C#/.NET application that simulates a checkout process. It handles order validation, price calculations, discounts, inventory availability, order persistence, and order confirmation notifications.

## Project Structure

```text
Mini-Checkout-Engine/
├── src/
│   ├── Simulation.Checkout.Domain/
│   ├── Simulation.Checkout.Application/
│   └── Simulation.Checkout.Infrastructure/
│
├── tests/
│   └── Simulation.Checkout.Tests/
│       ├── CheckoutServiceTests.cs
│       ├── TestData/
│       │   ├── OrderFactory.cs
│       │   ├── CustomerFaker.cs
│       │   └── TestDataTests.cs
│       ├── ReceiptSnapshotTests.cs
│       └── ArchitectureTests.cs
│
├── coverage-report/
├── TESTING_NOTES.md
└── README.md
```

## Technologies & Tools
- C# / .NET
- xUnit
- FluentAssertions
- Moq
- AutoFixture
- Bogus
- Verify.Xunit
- NetArchTest.Rules
- Coverlet
- ReportGenerator

## Testing

The project includes tests covering:

- Business rules and checkout validation.
- Mocking dependencies and verifying interactions.
- Automated test data generation.
- Snapshot testing.
- Architecture rules.
- Code coverage analysis.

### Run Tests

From the repository root, run:

```bash
dotnet test
```

### Generate Code Coverage Report

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

Generate the HTML coverage report using ReportGenerator:

```bash
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html;TextSummary"
```

## Coverage Results

- **Overall Line Coverage:** 90.5%
- **Domain:** 100%
- **Application:** 95.8%

The Domain and Application layers both exceed the required 80% line coverage target.

## Notes

See [TESTING_NOTES.md](TESTING_NOTES.md) for additional testing details and observations.