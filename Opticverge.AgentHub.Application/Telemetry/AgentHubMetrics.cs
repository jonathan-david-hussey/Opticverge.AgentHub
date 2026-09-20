using System.Diagnostics.Metrics;

namespace Opticverge.AgentHub.Application.Telemetry;

public sealed class AgentHubMetrics : IDisposable
{
    public const string MeterName = "Opticverge.AgentHub";

    private readonly Meter _meter = new(MeterName);

    public AgentHubMetrics()
    {
        AgentRunsStarted = _meter.CreateCounter<long>("agenthub.agent_runs.started");
        EvaluationRuns = _meter.CreateCounter<long>("agenthub.evaluation.runs");
        ProviderRoutingDecisions = _meter.CreateCounter<long>("agenthub.provider.routing_decisions");
        McpToolInvocations = _meter.CreateCounter<long>("agenthub.mcp.tool_invocations");
    }

    public Counter<long> AgentRunsStarted { get; }

    public Counter<long> EvaluationRuns { get; }

    public Counter<long> ProviderRoutingDecisions { get; }

    public Counter<long> McpToolInvocations { get; }

    public void Dispose() => _meter.Dispose();
}
