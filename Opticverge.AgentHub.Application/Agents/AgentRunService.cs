using Opticverge.AgentHub.Application.Providers;
using Opticverge.AgentHub.Domain.Agents;
using Opticverge.AgentHub.Domain.Eventing;
using Opticverge.AgentHub.Domain.Providers;

namespace Opticverge.AgentHub.Application.Agents;

public sealed class AgentRunService(IAgentRegistry registry, IProviderRouter providerRouter, TimeProvider timeProvider)
{
    public AgentRunRequestResult RequestRun(string agentId, string requestedBy)
    {
        var agent = registry.GetAgent(agentId);
        if (agent is null) return AgentRunRequestResult.NotFound(agentId);

        var routing = providerRouter.SelectProvider(agent);
        if (routing is null) return AgentRunRequestResult.NoProvider(agentId);

        var now = timeProvider.GetUtcNow();
        var run = new AgentRun(
            $"run-{Guid.NewGuid():N}",
            agent.Id,
            requestedBy,
            routing.ProviderId,
            AgentRunStatus.Requested,
            now,
            [
                new AgentRunStep("route-provider", AgentRunStepStatus.Completed, now, now, routing.Reason),
                new AgentRunStep("publish-request", AgentRunStepStatus.Started, now, null, AgentHubTopics.AgentRunRequested)
            ],
            new AgentRunCost(0, 0, 0));

        var envelope = new EventEnvelope<AgentRun>(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            run.RunId,
            now,
            1,
            AgentHubTopics.AgentRunRequested,
            run);

        return AgentRunRequestResult.Success(run, envelope, routing);
    }
}

public sealed record AgentRunRequestResult(
    bool Accepted,
    string? Error,
    AgentRun? Run,
    EventEnvelope<AgentRun>? Event,
    ProviderRoutingDecision? RoutingDecision)
{
    public static AgentRunRequestResult Success(
        AgentRun run,
        EventEnvelope<AgentRun> envelope,
        ProviderRoutingDecision routingDecision)
    {
        return new AgentRunRequestResult(true, null, run, envelope, routingDecision);
    }

    public static AgentRunRequestResult NotFound(string agentId)
    {
        return new AgentRunRequestResult(false, $"Agent '{agentId}' was not found.", null, null, null);
    }

    public static AgentRunRequestResult NoProvider(string agentId)
    {
        return new AgentRunRequestResult(false, $"No enabled healthy provider can run agent '{agentId}'.", null, null, null);
    }
}
