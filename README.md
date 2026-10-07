# QA Automation Mock Project

This repository contains the complete Quality Assurance (QA) automation suite and infrastructure for the FPT Academy Team 2 Mock Project. The target application is a banking/financial system.

## Project Structure

This project is divided into three main components:

1. **`e2e_automation/`**: Contains End-to-End (E2E) test suites, including a robust C# / .NET UI Automation framework and K6 performance testing scripts.
2. **`team2uiautomation/`**: (API Automation) Contains an extensive Apache JMeter test suite for API regression, functional, and data-driven testing.
3. **`infra_repo/`**: Contains the Infrastructure-as-Code (Docker Compose) for deploying the observability and metrics stack (InfluxDB and Grafana) to monitor test execution and application performance.

## Getting Started

Please refer to the individual `README.md` files in each sub-directory for detailed instructions on architecture, setup, and execution.

- [E2E Automation README](./e2e_automation/README.md)
- [API Automation README](./team2uiautomation/README.md)
- [Infrastructure README](./infra_repo/README.md)

## Security Note

Sensitive data such as Web URLs, session keys, passwords, and database credentials should never be committed to this repository. Please use `.env` files, `appsettings.json`, or CI/CD environment variables to manage configuration.

