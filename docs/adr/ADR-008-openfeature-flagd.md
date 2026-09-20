# ADR-008: OpenFeature With flagd

## Context

Experimental agents, provider enablement, routing policies, and expensive evaluation features need controlled rollout and rollback.

## Decision

Use OpenFeature abstractions with flagd as the local development provider.

## Alternatives Considered

- Appsettings flags only: requires redeploy/restart for many changes.
- Vendor-specific feature flags: unnecessary for the portfolio foundation.

## Consequences

Application code should not depend directly on flagd APIs.

## Success Criteria

Provider routing changes when flags change and disabled providers/agents are not selected.
