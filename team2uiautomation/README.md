# API Automation Suite (JMeter)

This directory contains an extensive API automation test suite built for testing the banking application's REST APIs.

## Architecture

- **Tool:** Apache JMeter 5.6.3
- **Design Pattern:** Modular Test Fragments. The tests are designed to be highly reusable and maintainable.
- **Structure:**
  - **`modules/`**: Contains reusable JMX fragments (e.g., login, register, delete user, approve account).
  - **`scripts/`**: Contains the actual test scenarios which combine modules using `IncludeController`.
  - **`test-data/`**: Contains CSV files for Data-Driven Testing (DDT), especially for negative test cases.
- **Master Plan:** `api_regression.jmx` acts as the orchestrator, running all scenario scripts across multiple Thread Groups.

## Features

- **Data-Driven Testing (DDT):** Extensively uses CSV Data Set Configs to validate various positive and negative scenarios (e.g., invalid logins, unauthorized transfers).
- **Observability Integration:** Uses JMeter's BackendListener to stream real-time test execution metrics directly into the InfluxDB instance provisioned by the `infra_repo`.
- **Assertions:** Utilizes JSONPath, JMESPath, and Response Assertions to rigorously validate API responses.
