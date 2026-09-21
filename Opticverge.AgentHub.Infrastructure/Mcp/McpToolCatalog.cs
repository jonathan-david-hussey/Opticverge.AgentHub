using Opticverge.AgentHub.Domain.Security;

namespace Opticverge.AgentHub.Infrastructure.Mcp;

public sealed record McpToolDescriptor(
    string Name,
    string Description,
    string RequiredPolicy,
    bool IsPrivileged,
    IReadOnlyDictionary<string, string> InputSchema);

public sealed class McpToolCatalog
{
    private static readonly McpToolDescriptor[] Tools =
    [
        new("list_agents", "List available AgentHub agents.", AgentHubPolicies.RunAgents, false, EmptySchema()),
        new("get_agent", "Get an agent definition by id.", AgentHubPolicies.RunAgents, false, IdSchema("agentId")),
        new("run_agent", "Request an agent run with idempotent command handling.", AgentHubPolicies.RunAgents, true, IdSchema("agentId")),
        new("get_agent_run", "Read agent run status and cost details.", AgentHubPolicies.ViewCostData, false, IdSchema("runId")),
        new("get_agent_run_timeline", "Read replayable timeline events for a run.", AgentHubPolicies.ReplayEvents, true, IdSchema("runId")),
        new("get_agent_cost_summary", "Read provider and run cost summaries.", AgentHubPolicies.ViewCostData, false, EmptySchema()),
        new("get_provider_health", "Read provider health and routing evidence.", AgentHubPolicies.RunAgents, false, EmptySchema()),
        new("get_readiness_summary", "Read AI engineering readiness evidence.", AgentHubPolicies.RunAgents, false, EmptySchema()),
        new("run_readiness_scan", "Request a readiness scan.", AgentHubPolicies.RunAgents, true, IdSchema("repository")),
        new("get_evaluation_result", "Read a prior evaluation result.", AgentHubPolicies.ViewCostData, false, IdSchema("evaluationRunId")),
        new("compare_provider_benchmarks", "Compare provider benchmark evidence.", AgentHubPolicies.ViewCostData, false, EmptySchema()),
        new("replay_agent_run", "Replay an agent run from event history.", AgentHubPolicies.ReplayEvents, true, IdSchema("runId"))
    ];

    public IReadOnlyList<McpToolDescriptor> ListTools()
    {
        return Tools;
    }

    public McpToolDescriptor? GetTool(string name)
    {
        return Tools.FirstOrDefault(tool => string.Equals(tool.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyDictionary<string, string> EmptySchema()
    {
        return new Dictionary<string, string>();
    }

    private static IReadOnlyDictionary<string, string> IdSchema(string propertyName)
    {
        return new Dictionary<string, string> { [propertyName] = "string" };
    }
}
