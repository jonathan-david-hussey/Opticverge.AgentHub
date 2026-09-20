using Opticverge.AgentHub.Domain.Agents;
using Opticverge.AgentHub.Domain.Providers;

namespace Opticverge.AgentHub.Application.Providers;

public sealed class ProviderRouter(IProviderCatalog providerCatalog, TimeProvider timeProvider) : IProviderRouter
{
    public ProviderRoutingDecision? SelectProvider(AgentDefinition agent)
    {
        var selected = providerCatalog
            .ListProviders()
            .Where(provider =>
                provider.IsEnabled &&
                provider.IsHealthy &&
                provider.Capabilities.Contains(agent.Capability))
            .OrderByDescending(provider => provider.EvaluationScore)
            .ThenBy(provider => provider.EstimatedCostPerThousandTokensUsd)
            .FirstOrDefault();

        if (selected is null)
        {
            return null;
        }

        return new ProviderRoutingDecision(
            selected.Id,
            agent.Id,
            "Selected highest-scoring healthy provider with matching capability, then lowest estimated token cost.",
            timeProvider.GetUtcNow(),
            new Dictionary<string, string>
            {
                ["provider.kind"] = selected.Kind.ToString(),
                ["provider.score"] = selected.EvaluationScore.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture),
                ["provider.costPerThousandTokensUsd"] = selected.EstimatedCostPerThousandTokensUsd.ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture)
            });
    }
}
