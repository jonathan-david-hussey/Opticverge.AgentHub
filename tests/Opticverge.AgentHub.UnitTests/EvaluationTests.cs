using Opticverge.AgentHub.Evaluation.Datasets;
using Opticverge.AgentHub.Evaluation.Evaluators;

namespace Opticverge.AgentHub.UnitTests;

public sealed class EvaluationTests
{
    [Fact]
    public void Deterministic_evaluation_passes_when_text_tools_and_policy_match()
    {
        var dataset = new EvaluationDataset(
            "demo",
            "1.0",
            [
                new(
                    "case-1",
                    "Prompt",
                    "Expected",
                    [new ExpectedToolCall("list_agents", new Dictionary<string, string>())],
                    ["replay_agent_run"],
                    [])
            ]);
        var candidate = new EvaluationCandidate(
            "case-1",
            "expected",
            [new ExpectedToolCall("list_agents", new Dictionary<string, string>())],
            [],
            TimeSpan.FromMilliseconds(50),
            10,
            5,
            0.001m);
        var runner = new EvaluationRunner(new DeterministicEvaluationScorer(), TimeProvider.System);

        var result = runner.Run(dataset, [candidate]);

        Assert.Equal(1, result.TaskSuccessRate);
        Assert.Equal(1, result.ToolAccuracy);
        Assert.Equal(0, result.PolicyFailures);
    }

    [Fact]
    public void Baseline_comparison_reports_regression_when_thresholds_are_missed()
    {
        var baseline = new EvaluationBaseline("demo", "1.0", 0.9, 0.9, 0, 0.01m, TimeSpan.FromMilliseconds(100));
        var result = new EvaluationRunResult(
            "demo",
            "1.0",
            DateTimeOffset.UtcNow,
            [new EvaluationCaseResult("case-1", false, 0.5, 1, TimeSpan.FromMilliseconds(150), 0.02m, [])]);

        var comparison = new BaselineComparer().Compare(baseline, result);

        Assert.False(comparison.Passed);
        Assert.Contains(comparison.Findings, finding => finding.Contains("Task success", StringComparison.Ordinal));
        Assert.Contains(comparison.Findings, finding => finding.Contains("Policy failures", StringComparison.Ordinal));
    }
}
