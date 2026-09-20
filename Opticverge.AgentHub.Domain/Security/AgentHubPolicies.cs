namespace Opticverge.AgentHub.Domain.Security;

public static class AgentHubPolicies
{
    public const string RunAgents = "agenthub.run-agents";
    public const string ViewCostData = "agenthub.view-cost-data";
    public const string ReplayEvents = "agenthub.replay-events";
    public const string ManageProviderSettings = "agenthub.manage-provider-settings";
    public const string InvokePrivilegedMcpTools = "agenthub.invoke-privileged-mcp-tools";
}
