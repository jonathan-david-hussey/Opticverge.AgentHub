# ADR-009: k6 For Repeatable Performance Evidence

## Context

The portfolio needs measurable latency, throughput, failure rate, provider degradation, Redis degradation, Kafka lag, and cost-under-load evidence.

## Decision

Use k6 scenarios stored in source control and runnable locally through Aspire.

## Alternatives Considered

- Manual browser or curl testing: not repeatable enough.
- Full load platform first: too heavy for the initial implementation.

## Consequences

Performance numbers must be generated from real runs and stored as evidence, not typed manually into docs.

## Success Criteria

Smoke thresholds run in CI or nightly jobs, and fuller scenarios produce baseline artifacts.
