# ADR-010: Keycloak For Local Identity

## Context

AgentHub needs local OIDC/JWT identity with roles for admin, platform engineer, and viewer scenarios.

## Decision

Use Keycloak in Aspire local development with a committed realm import for non-secret configuration.

## Alternatives Considered

- No identity: conflicts with secured agent, replay, cost, and provider operations.
- Cloud identity provider: unnecessary for local portfolio demos.

## Consequences

Secrets remain in user secrets or environment variables, and preview Aspire Keycloak usage is isolated to AppHost composition.

## Success Criteria

API and MCP endpoints reject anonymous privileged calls and accept authorized tokens with mapped AgentHub roles.
