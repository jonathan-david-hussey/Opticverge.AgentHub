# ADR-006: Separate Agent Evaluation From Software Tests

## Context

Unit and integration tests cannot fully verify nondeterministic agent behavior, tool selection, policy compliance, latency, and cost.

## Decision

Create a separate evaluation subsystem with versioned datasets, deterministic evaluators, optional semantic/LLM judges, baselines, and regression comparison.

## Alternatives Considered

- Only unit tests: misses prompt/tool regressions.
- Always use LLM-as-judge: expensive and nondeterministic for CI.

## Consequences

Paid-provider evaluation is opt-in; deterministic evaluation runs by default.

## Success Criteria

Evaluation artifacts are machine-readable, human-readable, and able to block regressions in CI.
