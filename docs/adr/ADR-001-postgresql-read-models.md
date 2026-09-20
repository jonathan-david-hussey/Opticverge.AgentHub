# ADR-001: PostgreSQL For Durable Read Models

## Context

AgentHub needs durable query models for agent runs, notifications, evaluation results, cost summaries, readiness scans, and replay metadata.

## Decision

Use PostgreSQL as the durable relational store for read/query models and operational records.

## Alternatives Considered

- SQLite: simpler locally, weaker portfolio signal for distributed app operations.
- Document database: flexible, but no current query shape requires it.

## Consequences

PostgreSQL becomes part of the Aspire local topology and integration-test surface.

## Success Criteria

Read models survive service restarts, health checks are observable, and tests can verify persisted evaluation/run evidence.
