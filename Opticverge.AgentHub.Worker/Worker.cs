using Opticverge.AgentHub.Application.Agents;
using Opticverge.AgentHub.Evaluation.Datasets;
using Opticverge.AgentHub.Evaluation.Evaluators;

namespace Opticverge.AgentHub.Worker;

public sealed class Worker(
    ILogger<Worker> logger,
    AgentRunService agentRunService,
    EvaluationRunner evaluationRunner) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var run = agentRunService.RequestRun("readiness-scanner", "worker@agenthub.local");
            if (run.Accepted && run.Run is not null)
                logger.LogInformation(
                    "Accepted portfolio worker run {RunId} for agent {AgentId} with provider {ProviderId}",
                    run.Run.RunId,
                    run.Run.AgentId,
                    run.Run.ProviderId);

            var dataset = new EvaluationDataset(
                "portfolio-smoke",
                "1.0.0",
                [
                    new EvaluationCase(
                        "readiness-summary",
                        "Summarize readiness",
                        "Repository has build, tests, boundaries, telemetry, and externalized secrets.",
                        [],
                        [],
                        [])
                ]);
            var candidate = new EvaluationCandidate(
                "readiness-summary",
                "Repository has build, tests, boundaries, telemetry, and externalized secrets.",
                [],
                [],
                TimeSpan.FromMilliseconds(120),
                25,
                18,
                0m);
            var evaluation = evaluationRunner.Run(dataset, [candidate]);
            logger.LogInformation(
                "Evaluation dataset {DatasetId} success rate {TaskSuccessRate:P0}",
                evaluation.DatasetId,
                evaluation.TaskSuccessRate);

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
