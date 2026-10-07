# Infrastructure & Observability Stack

This repository contains the configuration and deployment scripts for the monitoring and observability infrastructure used by the QA automation suites.

## Architecture

The infrastructure relies on a containerized stack managed by **Docker Compose**:

- **InfluxDB 2.0:** A time-series database used to store test metrics, results, and performance data over time (especially from JMeter and K6).
- **Grafana:** A visualization and dashboarding platform connected to InfluxDB to display real-time and historical test analytics.

### Deployment Flow
- Uses **GitLab CI/CD** to automate deployments.
- Authenticates via SSH to a remote VPS.
- Uses `rsync` to copy the configuration files (`docker-compose.yml`, etc.) to the target server.
- Executes `docker compose up -d` to provision/update the services.
- Persistent data is managed through Docker named volumes (`influxdb_data`, `influxdb_config`, `grafana_data`).

## Usage

This stack is entirely automated via the CI pipeline. To run it locally for testing:
1. Ensure Docker and Docker Compose are installed.
2. Provide a valid `.env` file with necessary secrets (username, password, org, bucket, tokens).
3. Run `docker compose up -d`.