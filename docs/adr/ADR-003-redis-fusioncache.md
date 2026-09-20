# ADR-003: Redis And FusionCache For Distributed Caching

## Context

Agent registry, provider metadata, readiness summaries, scorecards, cost aggregates, and short-lived permissions need fast reads with cross-service invalidation.

## Decision

Use Redis as the shared cache resource and FusionCache as the app-facing cache pattern: L1 memory, Redis L2, fail-safe, stampede protection, and explicit key prefixes.

## Alternatives Considered

- Memory-only cache: insufficient for API/Web/Worker coordination.
- Multiple Redis instances: useful later, unnecessary for the portfolio foundation.

## Consequences

All keys use `agenthub:{area}:{id}` and production notes should split Redis workloads if throughput grows.

## Success Criteria

Cache behavior is observable, degraded Redis scenarios can be tested, and invalidation is triggered by Kafka events.
