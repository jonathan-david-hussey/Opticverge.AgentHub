using Opticverge.AgentHub.Domain.Agents;

namespace Opticverge.AgentHub.Application.Agents;

public interface IAgentRegistry
{
    IReadOnlyList<AgentDefinition> ListAgents();

    AgentDefinition? GetAgent(string agentId);
}
