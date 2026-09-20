using Opticverge.AgentHub.Evaluation.Datasets;

namespace Opticverge.AgentHub.Evaluation.Evaluators;

public interface IEvaluationScorer
{
    EvaluationCaseResult Score(EvaluationCase evaluationCase, EvaluationCandidate candidate);
}

public sealed class DeterministicEvaluationScorer : IEvaluationScorer
{
    public EvaluationCaseResult Score(EvaluationCase evaluationCase, EvaluationCandidate candidate)
    {
        var findings = new List<string>();
        var textMatches = string.Equals(
            evaluationCase.ExpectedText.Trim(),
            candidate.ActualText.Trim(),
            StringComparison.OrdinalIgnoreCase);

        if (!textMatches)
        {
            findings.Add("Expected text did not match candidate output.");
        }

        var expectedCalls = evaluationCase.ExpectedToolCalls;
        var matchingCalls = expectedCalls.Count(expected =>
            candidate.ToolCalls.Any(actual =>
                string.Equals(expected.ToolName, actual.ToolName, StringComparison.OrdinalIgnoreCase) &&
                expected.Arguments.All(argument =>
                    actual.Arguments.TryGetValue(argument.Key, out var actualValue) &&
                    string.Equals(argument.Value, actualValue, StringComparison.OrdinalIgnoreCase))));

        var forbiddenCalls = candidate.ToolCalls
            .Where(call => evaluationCase.ForbiddenToolNames.Contains(call.ToolName, StringComparer.OrdinalIgnoreCase))
            .Select(call => call.ToolName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var forbiddenCall in forbiddenCalls)
        {
            findings.Add($"Forbidden tool call '{forbiddenCall}' was used.");
        }

        var toolAccuracy = expectedCalls.Count == 0 ? 1 : matchingCalls / (double)expectedCalls.Count;
        var policyFailures = candidate.PolicyViolations.Count;
        if (policyFailures > 0)
        {
            findings.Add($"{policyFailures} policy violation(s) were reported.");
        }

        var success = textMatches && toolAccuracy >= 1 && forbiddenCalls.Length == 0 && policyFailures == 0;
        if (success)
        {
            findings.Add("Case passed deterministic checks.");
        }

        return new EvaluationCaseResult(
            evaluationCase.Id,
            success,
            toolAccuracy,
            policyFailures,
            candidate.Latency,
            candidate.EstimatedCostUsd,
            findings);
    }
}
