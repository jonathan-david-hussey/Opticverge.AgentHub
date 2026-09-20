# ADR-007: MCP As External Agent Contract

## Context

Developer assistants and external agents need controlled access to AgentHub capabilities.

## Decision

Expose selected capabilities through MCP-style tools with schemas, authorization, correlation IDs, idempotency, tracing, and audit logging.

## Alternatives Considered

- Expose internal APIs directly: too broad and harder to audit.
- No external tool contract: weakens the AI engineering enablement story.

## Consequences

Privileged tools such as replay and expensive evaluations require elevated policy.

## Success Criteria

Tool metadata is discoverable, unauthorized privileged calls are rejected, and invocations produce trace/audit evidence.
