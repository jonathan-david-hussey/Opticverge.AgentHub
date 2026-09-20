using Opticverge.AgentHub.Domain.Readiness;

namespace Opticverge.AgentHub.Application.Readiness;

public sealed class ReadinessService(TimeProvider timeProvider)
{
    public ReadinessSummary SummarizeRepository(string repositoryName)
    {
        ReadinessCheck[] checks =
        [
            new("build", "Build runs non-interactively", ReadinessCheckStatus.Passed, "dotnet build is wired in CI.", 15),
            new("tests", "Tests are present", ReadinessCheckStatus.Passed, "Unit and Aspire integration test projects are part of the solution.", 15),
            new("boundaries", "Architecture boundaries are visible", ReadinessCheckStatus.Passed, "Domain, Application, Infrastructure, Web, API, Worker, MCP, and Evaluation projects are separated.", 15),
            new("observability", "Telemetry is configured", ReadinessCheckStatus.Passed, "ServiceDefaults enables OpenTelemetry logs, metrics, traces, health, and service discovery.", 15),
            new("secrets", "Secrets are externalized", ReadinessCheckStatus.Passed, "Provider credentials and identity secrets are expected through user secrets or environment variables.", 10),
            new("runtime-diagnostics", "Runtime diagnostics are available", ReadinessCheckStatus.Warning, "Aspire dashboard, Seq, Prometheus, Grafana, and MCP are scaffolded; full persisted diagnostics are future work.", 10)
        ];

        var earned = checks
            .Where(check => check.Status is ReadinessCheckStatus.Passed)
            .Sum(check => check.Weight);
        var possible = checks.Sum(check => check.Weight);
        var score = (int)Math.Round(100m * earned / possible, MidpointRounding.AwayFromZero);

        return new ReadinessSummary(
            repositoryName,
            score,
            timeProvider.GetUtcNow(),
            checks,
            ["Persist provider benchmark artifacts from real runs.", "Replace in-memory portfolio adapters with durable stores during Phase 2."]);
    }
}
