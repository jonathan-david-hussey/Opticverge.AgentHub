using Opticverge.AgentHub.Domain.Agents;
using Opticverge.AgentHub.Domain.Providers;

namespace Opticverge.AgentHub.Application.Providers;

public interface IProviderCatalog
{
    IReadOnlyList<ProviderProfile> ListProviders();
}

public interface IProviderRouter
{
    ProviderRoutingDecision? SelectProvider(AgentDefinition agent);
}
