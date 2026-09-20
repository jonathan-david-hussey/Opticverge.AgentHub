# ADR-002: Kafka For Event Propagation And Replay

## Context

The platform needs event propagation, replayable agent timelines, notifications, and evaluation/provider benchmark events.

## Decision

Use Kafka topics with a common event envelope for domain notifications and replay history.

## Alternatives Considered

- In-memory queues: useful for unit tests only.
- A second broker: explicitly out of scope until a concrete requirement appears.

## Consequences

Events must include event id, correlation id, causation id, aggregate id, timestamp, schema version, topic, and typed payload.

## Success Criteria

Agent runs publish events, workers consume them, lag is measured, and replay reconstructs expected timelines.
