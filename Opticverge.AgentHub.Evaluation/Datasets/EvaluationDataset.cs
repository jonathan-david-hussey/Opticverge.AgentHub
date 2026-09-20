namespace Opticverge.AgentHub.Evaluation.Datasets;

public sealed record EvaluationDataset(
    string Id,
    string Version,
    IReadOnlyList<EvaluationCase> Cases);

public sealed record EvaluationCase(
    string Id,
    string Prompt,
    string ExpectedText,
    IReadOnlyList<ExpectedToolCall> ExpectedToolCalls,
    IReadOnlyList<string> ForbiddenToolNames,
    IReadOnlyList<string> RequiredPolicies);

public sealed record ExpectedToolCall(
    string ToolName,
    IReadOnlyDictionary<string, string> Arguments);

public sealed record EvaluationCandidate(
    string CaseId,
    string ActualText,
    IReadOnlyList<ExpectedToolCall> ToolCalls,
    IReadOnlyList<string> PolicyViolations,
    TimeSpan Latency,
    int InputTokens,
    int OutputTokens,
    decimal EstimatedCostUsd);
