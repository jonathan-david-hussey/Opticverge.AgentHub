namespace Opticverge.AgentHub.Domain.Agents;

public sealed record AgentRun(
    string RunId,
    string AgentId,
    string RequestedBy,
    string ProviderId,
    AgentRunStatus Status,
    DateTimeOffset RequestedAt,
    IReadOnlyList<AgentRunStep> Steps,
    AgentRunCost Cost);

public sealed record AgentRunStep(
    string Name,
    AgentRunStepStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    string? Detail);

public sealed record AgentRunCost(
    int InputTokens,
    int OutputTokens,
    decimal EstimatedUsd);

public enum AgentRunStatus
{
    Requested,
    Running,
    Completed,
    Failed,
    Replayed
}

public enum AgentRunStepStatus
{
    Started,
    Completed,
    Failed
}
