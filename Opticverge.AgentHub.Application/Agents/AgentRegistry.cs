using Opticverge.AgentHub.Domain.Agents;
using Opticverge.AgentHub.Domain.Security;

namespace Opticverge.AgentHub.Application.Agents;

public sealed class AgentRegistry : IAgentRegistry
{
    private static readonly AgentDefinition[] Agents =
    [
        new(
            "pr-reviewer",
            "PR Reviewer",
            "Reviews code changes for reliability, tests, security, and maintainability evidence.",
            AgentCapability.CodeReview,
            false,
            [AgentHubPolicies.RunAgents]),
        new(
            "readiness-scanner",
            "AI Readiness Scanner",
            "Scores whether a repository is ready for productive AI-assisted engineering.",
            AgentCapability.ReadinessScanning,
            false,
            [AgentHubPolicies.RunAgents]),
        new(
            "provider-comparison",
            "Provider Comparison",
            "Runs a shared evaluation dataset across enabled providers and reports quality, cost, and latency trade-offs.",
            AgentCapability.ProviderBenchmarking,
            true,
            [AgentHubPolicies.RunAgents, AgentHubPolicies.ViewCostData]),
        new(
            "run-replay",
            "Agent Run Replay",
            "Replays a previously recorded run from durable event history.",
            AgentCapability.RunReplay,
            true,
            [AgentHubPolicies.ReplayEvents])
    ];

    public IReadOnlyList<AgentDefinition> ListAgents() => Agents;

    public AgentDefinition? GetAgent(string agentId) =>
        Agents.FirstOrDefault(agent => string.Equals(agent.Id, agentId, StringComparison.OrdinalIgnoreCase));
}
