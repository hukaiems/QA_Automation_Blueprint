# E2E Automation Suite

This repository contains the End-to-End automation tests, focusing on UI regression and system performance.

## Architecture & Frameworks

The E2E suite is split into two primary areas:

### 1. UI Automation (`ui-automation/`)
- **Framework:** C# .NET 10.0, NUnit, and Selenium WebDriver.
- **Design Pattern:** Page Object Model (POM) to abstract UI interactions from test logic.
- **Capabilities:** Parallel execution across multiple browsers (Chrome, Edge) via Selenium Grid. Data-driven testing is supported via Excel utilities.
- **Test Categories:** Smoke Tests, Regression Tests, and specific scenario executions (e.g., Deposit, Withdraw, Transfer, Register/Login).

### 2. Performance Testing (`performance/`)
- **Framework:** K6 (JavaScript)
- **Purpose:** Load and performance testing of critical API endpoints (e.g., Transfer, User Auth).
- **Architecture:** Scenarios are defined in JavaScript, utilizing dynamic test data generation scripts and SharedArrays for data-driven load simulation. Integrates with k6-reporter to generate HTML summary reports.

## CI/CD Integration

The suite uses GitLab CI (`.gitlab-ci.yml`) to orchestrate multi-stage pipelines:
- `ui_api_smoke`: Runs Smoke tests on Chrome.
- `trigger_jmeter_api_tests`: Triggers the downstream API automation pipeline.
- `K6_performance`: Executes load tests against the target environment.
- `ui_regression`: Runs full cross-browser UI regression tests in parallel.

