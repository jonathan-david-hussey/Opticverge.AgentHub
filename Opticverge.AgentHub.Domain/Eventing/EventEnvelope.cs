namespace Opticverge.AgentHub.Domain.Eventing;

public sealed record EventEnvelope<TPayload>(
    Guid EventId,
    Guid CorrelationId,
    Guid? CausationId,
    string AggregateId,
    DateTimeOffset OccurredAt,
    int SchemaVersion,
    string Topic,
    TPayload Payload);

public static class AgentHubTopics
{
    public const string AgentRunRequested = "agent-run-requested";
    public const string AgentRunStepStarted = "agent-run-step-started";
    public const string AgentRunStepCompleted = "agent-run-step-completed";
    public const string AgentRunFailed = "agent-run-failed";
    public const string ReadinessScanRequested = "readiness-scan-requested";
    public const string VerificationEvidenceRecorded = "verification-evidence-recorded";
    public const string AiUsageRecorded = "ai-usage-recorded";
    public const string AgentRunReplayRequested = "agent-run-replay-requested";
    public const string NotificationRequested = "notification-requested";
    public const string EvaluationRunRequested = "evaluation-run-requested";
    public const string EvaluationCaseCompleted = "evaluation-case-completed";
    public const string EvaluationRunCompleted = "evaluation-run-completed";
    public const string ProviderBenchmarkCompleted = "provider-benchmark-completed";
    public const string ProviderRoutingDecisionRecorded = "provider-routing-decision-recorded";
    public const string FeatureFlagEvaluated = "feature-flag-evaluated";
    public const string McpToolInvoked = "mcp-tool-invoked";
    public const string ReadinessRecommendationRecorded = "readiness-recommendation-recorded";
}
