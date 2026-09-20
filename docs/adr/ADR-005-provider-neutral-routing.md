# ADR-005: Provider-Neutral Agent Execution

## Context

Provider choice should be explainable and influenced by capability, health, cost, feature flags, evaluation score, and policy.

## Decision

Use a provider router service outside agent code. Record routing decisions as evidence.

## Alternatives Considered

- Agent-specific provider selection: duplicates policy logic.
- Configuration-only routing: lacks runtime health and evaluation evidence.

## Consequences

Every provider decision should include reason and evidence fields.

## Success Criteria

Disabled or unhealthy providers are not selected, fallback decisions are observable, and cost/quality trade-offs can be explained.
