using Opticverge.AgentHub.Application.Providers;
using Opticverge.AgentHub.Domain.Agents;
using Opticverge.AgentHub.Domain.Providers;

namespace Opticverge.AgentHub.Infrastructure.Agents;

public sealed class InMemoryProviderCatalog : IProviderCatalog
{
    private static readonly ProviderProfile[] Providers =
    [
        new(
            "openai",
            "OpenAI Responses",
            ProviderKind.OpenAi,
            true,
            true,
            0.005m,
            0.94,
            new HashSet<AgentCapability>
            {
                AgentCapability.CodeReview,
                AgentCapability.InvoiceAssistance,
                AgentCapability.ReadinessScanning,
                AgentCapability.ProviderBenchmarking
            }),
        new(
            "github-models",
            "GitHub Models",
            ProviderKind.GitHubModels,
            true,
            true,
            0.003m,
            0.9,
            new HashSet<AgentCapability>
            {
                AgentCapability.CodeReview,
                AgentCapability.ReadinessScanning,
                AgentCapability.ProviderBenchmarking
            }),
        new(
            "ollama",
            "Ollama Local",
            ProviderKind.Ollama,
            true,
            true,
            0m,
            0.78,
            new HashSet<AgentCapability>
            {
                AgentCapability.CodeReview,
                AgentCapability.ReadinessScanning
            }),
        new(
            "deterministic-test",
            "Deterministic Test Provider",
            ProviderKind.DeterministicTest,
            true,
            true,
            0m,
            1,
            new HashSet<AgentCapability>
            {
                AgentCapability.CodeReview,
                AgentCapability.InvoiceAssistance,
                AgentCapability.ReadinessScanning,
                AgentCapability.ProviderBenchmarking,
                AgentCapability.RunReplay
            })
    ];

    public IReadOnlyList<ProviderProfile> ListProviders()
    {
        return Providers;
    }
}
