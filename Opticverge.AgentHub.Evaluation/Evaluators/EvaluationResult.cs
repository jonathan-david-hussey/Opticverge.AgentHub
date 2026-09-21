namespace Opticverge.AgentHub.Evaluation.Evaluators;

public sealed record EvaluationRunResult(
    string DatasetId,
    string DatasetVersion,
    DateTimeOffset CompletedAt,
    IReadOnlyList<EvaluationCaseResult> Cases)
{
    public double TaskSuccessRate => Cases.Count == 0 ? 0 : Cases.Count(item => item.Success) / (double)Cases.Count;

    public double ToolAccuracy => Cases.Count == 0 ? 0 : Cases.Average(item => item.ToolAccuracy);

    public int PolicyFailures => Cases.Sum(item => item.PolicyFailures);

    public decimal EstimatedCostUsd => Cases.Sum(item => item.EstimatedCostUsd);

    public TimeSpan P95Latency => Percentile(Cases.Select(item => item.Latency).Order().ToArray(), 0.95);

    private static TimeSpan Percentile(TimeSpan[] values, double percentile)
    {
        if (values.Length == 0) return TimeSpan.Zero;

        var index = (int)Math.Ceiling(percentile * values.Length) - 1;
        return values[Math.Clamp(index, 0, values.Length - 1)];
    }
}

public sealed record EvaluationCaseResult(
    string CaseId,
    bool Success,
    double ToolAccuracy,
    int PolicyFailures,
    TimeSpan Latency,
    decimal EstimatedCostUsd,
    IReadOnlyList<string> Findings);

public sealed record EvaluationBaseline(
    string DatasetId,
    string DatasetVersion,
    double MinimumTaskSuccessRate,
    double MinimumToolAccuracy,
    int MaximumPolicyFailures,
    decimal MaximumCostIncreaseUsd,
    TimeSpan MaximumP95Latency);

public sealed record EvaluationComparison(
    bool Passed,
    IReadOnlyList<string> Findings);
