namespace Opticverge.AgentHub.Evaluation.Evaluators;

public sealed class BaselineComparer
{
    public EvaluationComparison Compare(EvaluationBaseline baseline, EvaluationRunResult candidate)
    {
        var findings = new List<string>();

        if (candidate.TaskSuccessRate < baseline.MinimumTaskSuccessRate)
            findings.Add($"Task success {candidate.TaskSuccessRate:P0} is below baseline {baseline.MinimumTaskSuccessRate:P0}.");

        if (candidate.ToolAccuracy < baseline.MinimumToolAccuracy)
            findings.Add($"Tool accuracy {candidate.ToolAccuracy:P0} is below baseline {baseline.MinimumToolAccuracy:P0}.");

        if (candidate.PolicyFailures > baseline.MaximumPolicyFailures)
            findings.Add($"Policy failures {candidate.PolicyFailures} exceed baseline {baseline.MaximumPolicyFailures}.");

        if (candidate.EstimatedCostUsd > baseline.MaximumCostIncreaseUsd)
            findings.Add($"Estimated cost {candidate.EstimatedCostUsd:C} exceeds configured maximum {baseline.MaximumCostIncreaseUsd:C}.");

        if (candidate.P95Latency > baseline.MaximumP95Latency)
            findings.Add($"P95 latency {candidate.P95Latency.TotalMilliseconds:0}ms exceeds baseline {baseline.MaximumP95Latency.TotalMilliseconds:0}ms.");

        if (findings.Count == 0) findings.Add("Candidate evaluation satisfies the configured baseline.");

        return new EvaluationComparison(findings.Count == 1 && findings[0].StartsWith("Candidate", StringComparison.Ordinal), findings);
    }
}
