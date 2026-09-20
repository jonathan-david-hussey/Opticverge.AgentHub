namespace Opticverge.AgentHub.Domain.Readiness;

public sealed record ReadinessSummary(
    string RepositoryName,
    int Score,
    DateTimeOffset EvaluatedAt,
    IReadOnlyList<ReadinessCheck> Checks,
    IReadOnlyList<string> Recommendations);

public sealed record ReadinessCheck(
    string Id,
    string Name,
    ReadinessCheckStatus Status,
    string Evidence,
    int Weight);

public enum ReadinessCheckStatus
{
    Passed,
    Warning,
    Failed
}
