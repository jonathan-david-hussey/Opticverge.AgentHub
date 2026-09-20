# ADR-004: Microsoft Agent Framework And Microsoft.Extensions.AI

## Context

AgentHub should demonstrate provider-neutral orchestration rather than direct coupling to one model SDK.

## Decision

Keep orchestration behind application interfaces and use Microsoft Agent Framework and `Microsoft.Extensions.AI` for provider-facing adapters as implementation matures.

## Alternatives Considered

- Direct provider SDK calls in agents: faster initially, poor portability.
- Custom model abstraction only: more maintenance than needed.

## Consequences

Provider-specific logic must remain in infrastructure adapters.

## Success Criteria

OpenAI, GitHub Models, Ollama, and deterministic test providers can run through the same routing and evaluation contracts.
