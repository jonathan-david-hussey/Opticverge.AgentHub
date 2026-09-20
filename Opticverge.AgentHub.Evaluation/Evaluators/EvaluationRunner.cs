using Opticverge.AgentHub.Evaluation.Datasets;

namespace Opticverge.AgentHub.Evaluation.Evaluators;

public sealed class EvaluationRunner(IEvaluationScorer scorer, TimeProvider timeProvider)
{
    public EvaluationRunResult Run(EvaluationDataset dataset, IReadOnlyList<EvaluationCandidate> candidates)
    {
        var results = dataset.Cases
            .Select(evaluationCase =>
            {
                var candidate = candidates.FirstOrDefault(item => item.CaseId == evaluationCase.Id)
                    ?? EmptyCandidate(evaluationCase.Id);
                return scorer.Score(evaluationCase, candidate);
            })
            .ToArray();

        return new EvaluationRunResult(dataset.Id, dataset.Version, timeProvider.GetUtcNow(), results);
    }

    private static EvaluationCandidate EmptyCandidate(string caseId) =>
        new(caseId, string.Empty, [], ["candidate-missing"], TimeSpan.Zero, 0, 0, 0);
}
