# Opticverge.AgentHub

Opticverge.AgentHub is a .NET 10 Aspire portfolio application for an internal AI enablement hub. It is designed to show how an engineering platform team could register agents, route work across AI providers, evaluate agent behavior, track readiness evidence, expose controlled MCP tools, and operate the whole system with production-style observability.

The project is intentionally more than a chatbot. It is an early AgentHub foundation with:

- A Blazor dashboard and visual Agentflow-style builder.
- A secured API for agent registry, run requests, provider health, readiness, and evaluations.
- A secured MCP facade for external agents and developer assistants.
- Provider-neutral routing and deterministic evaluation primitives.
- Aspire orchestration for local infrastructure.
- CI validation, coverage, CodeQL, dependency review, and optional SonarQube analysis.

## Current Experience

Run the Blazor hub directly when you want to inspect the product surface without starting all Aspire infrastructure:

```bash
dotnet run --project Opticverge.AgentHub.Web/Opticverge.AgentHub.Web.csproj --urls http://localhost:5245
```

Then open:

- Dashboard: http://localhost:5245/
- Agents: http://localhost:5245/agents
- Visual builder: http://localhost:5245/agentflows

The visual builder currently presents portfolio-grade agentflow templates rather than a full drag-and-drop workflow editor. It is intended to communicate the Flowise/n8n-like direction: triggers, policy gates, provider routing, tools, evaluation, persistence, notifications, and observability.

## Prerequisites

- .NET SDK 10.0.x
- Docker or a compatible container runtime, for the full Aspire topology
- GitHub CLI only if you want to create or push the repository from the command line

The SDK version is pinned in `global.json`.

## Quick Start

Restore, build, and test:

```bash
dotnet restore Opticverge.AgentHub.sln
dotnet build Opticverge.AgentHub.sln
dotnet test Opticverge.AgentHub.sln --no-build
```

Run only the web dashboard:

```bash
dotnet run --project Opticverge.AgentHub.Web/Opticverge.AgentHub.Web.csproj --urls http://localhost:5245
```

Run the full Aspire app host:

```bash
dotnet run --project Opticverge.AgentHub.AppHost/Opticverge.AgentHub.AppHost.csproj
```

The Aspire host composes the app services and local infrastructure. Container startup may take time the first time images are pulled.

## Local Service URLs

Direct project launch profiles include:

- Web: http://localhost:5245
- API: http://localhost:5091
- AppHost: http://localhost:15271

Aspire-managed infrastructure includes fixed ports for selected tools:

- Keycloak: http://localhost:8080
- Kafka UI: http://localhost:8085
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000
- pgAdmin: http://localhost:5050
- MCP Inspector: http://localhost:6274

Some service ports can differ when launched through Aspire depending on the selected profile and resource configuration.

## Solution Layout

```text
Opticverge.AgentHub.AppHost          Aspire local orchestration
Opticverge.AgentHub.ServiceDefaults  OpenTelemetry, health, resilience, discovery
Opticverge.AgentHub.Web              Blazor dashboard, agent directory, visual builder
Opticverge.AgentHub.Api              Secured AgentHub HTTP API and SignalR hub
Opticverge.AgentHub.Mcp              Secured MCP-compatible facade
Opticverge.AgentHub.Worker           Background orchestration loop
Opticverge.AgentHub.Domain           Agent, provider, event, readiness, security contracts
Opticverge.AgentHub.Application      Registry, routing, readiness, run request services
Opticverge.AgentHub.Infrastructure   Provider catalog, cache plan, MCP tool catalog
Opticverge.AgentHub.Evaluation       Deterministic evaluation and baseline comparison
tests/                              Unit and lightweight Aspire composition tests
infra/                              Local Prometheus, Grafana, OTEL, Keycloak, flagd, k6 config
evaluation/                         Versioned datasets and baselines
docs/adr/                           Architecture decision records
```

## Agents

The current registered agents are:

- `pr-reviewer`: reviews code changes for reliability, tests, security, and maintainability evidence.
- `readiness-scanner`: scores whether a repository is ready for AI-assisted engineering.
- `provider-comparison`: compares providers across quality, cost, latency, and policy evidence.
- `run-replay`: represents replay of agent runs from durable event history.

Open `/agents` to inspect the registry and provider fit. Open `/agentflows` to inspect visual workflow templates for selected agents.

Agent execution is intentionally guarded behind API and MCP authorization policies. The Blazor pages show the hub and composition model without bypassing those policies.

## API And MCP

The API exposes secured endpoints for:

- Agent registry
- Agent run requests
- Provider health and routing evidence
- Readiness summaries
- Deterministic evaluations
- SignalR agent run updates

The MCP facade exposes controlled tool metadata and privileged invocation routing for external assistants. High-risk tools such as run replay and evaluation comparison are represented with explicit authorization policy metadata.

Local development defaults expect Keycloak-issued JWTs with the `agenthub_role` claim. The committed Keycloak realm is development-only and contains non-secret configuration.

## Evaluation

The evaluation subsystem is separate from normal software tests. It supports:

- Versioned datasets
- Deterministic exact/tool/policy scoring
- Baseline comparison
- Machine-readable artifacts

Seed artifacts live under:

```text
evaluation/datasets/
evaluation/baselines/
```

Paid-provider evaluation is not required for CI by default.

## Observability And Infrastructure

The Aspire host includes:

- PostgreSQL for durable read/query models
- Kafka for event propagation and replay history
- Redis for shared cache and SignalR/FusionCache-style coordination
- Seq for structured logs
- Keycloak for local identity
- Prometheus and Grafana for metrics
- OpenTelemetry Collector for telemetry ingestion
- flagd for feature flag scenarios
- k6 for performance smoke tests
- Kafka UI and MCP Inspector for development inspection

Some of these are currently scaffolded as local resources and configuration. Deeper persistence, cache invalidation, real provider calls, and end-to-end identity flows are the next implementation slices.

## CI

GitHub Actions workflow: `.github/workflows/ci.yml`

The pipeline runs:

- Restore
- Build in Release
- Tests in Release
- Coverage collection and report artifact upload
- CodeQL for C#
- Dependency review on pull requests
- Optional SonarQube analysis when `SONAR_TOKEN` and `SONAR_HOST_URL` are configured

There are no publish or deploy steps yet.

## Architecture Decisions

ADRs live in `docs/adr/` and cover the main design choices:

- PostgreSQL read models
- Kafka event history
- Redis and FusionCache-style caching
- Microsoft Agent Framework and `Microsoft.Extensions.AI`
- Provider-neutral routing
- Separate agent evaluation
- MCP tool contract
- OpenFeature and flagd
- k6 performance evidence
- Keycloak identity

## Development Notes

Run the full verification locally:

```bash
dotnet restore Opticverge.AgentHub.sln
dotnet build Opticverge.AgentHub.sln --configuration Release --no-restore
dotnet test Opticverge.AgentHub.sln --configuration Release --no-build --settings coverage.runsettings --collect:"XPlat Code Coverage" --logger trx --results-directory artifacts/test-results
```

Generated build, test, and coverage outputs are ignored by Git.

## Current Status

This repository is a working foundation, not a finished internal AI platform. The implemented pieces are meant to demonstrate architecture, delivery practices, agent registration, provider routing, visual workflow direction, deterministic evaluation, observability design, and CI readiness.

Next useful slices:

- Persist agent runs, evaluation results, readiness history, and cost records to PostgreSQL.
- Wire Kafka producers/consumers for real event history.
- Add one real AI provider adapter plus deterministic fallback.
- Add authenticated dashboard execution flows.
- Expand the visual builder from template inspection into editable workflows.
- Add end-to-end Keycloak token tests.
