using Opticverge.AgentHub.Application.Agents;
using Opticverge.AgentHub.Application.Providers;
using Opticverge.AgentHub.Domain.Agents;
using Opticverge.AgentHub.Domain.Providers;

namespace Opticverge.AgentHub.UnitTests;

public sealed class ProviderRouterTests
{
    [Fact]
    public void SelectProvider_prefers_highest_scoring_healthy_provider_with_matching_capability()
    {
        var router = new ProviderRouter(new TestProviderCatalog(), TimeProvider.System);
        var agent = new AgentDefinition("agent", "Agent", "Description", AgentCapability.CodeReview, false, []);

        var decision = router.SelectProvider(agent);

        Assert.NotNull(decision);
        Assert.Equal("best", decision.ProviderId);
        Assert.Contains("highest-scoring", decision.Reason, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class TestProviderCatalog : IProviderCatalog
    {
        public IReadOnlyList<ProviderProfile> ListProviders() =>
        [
            new("cheap", "Cheap", ProviderKind.Ollama, true, true, 0m, 0.7, new HashSet<AgentCapability> { AgentCapability.CodeReview }),
            new("down", "Down", ProviderKind.OpenAi, true, false, 0.01m, 1, new HashSet<AgentCapability> { AgentCapability.CodeReview }),
            new("best", "Best", ProviderKind.GitHubModels, true, true, 0.01m, 0.95, new HashSet<AgentCapability> { AgentCapability.CodeReview })
        ];
    }
}
