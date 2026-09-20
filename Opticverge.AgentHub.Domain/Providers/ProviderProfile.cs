using Opticverge.AgentHub.Domain.Agents;

namespace Opticverge.AgentHub.Domain.Providers;

public sealed record ProviderProfile(
    string Id,
    string DisplayName,
    ProviderKind Kind,
    bool IsEnabled,
    bool IsHealthy,
    decimal EstimatedCostPerThousandTokensUsd,
    double EvaluationScore,
    IReadOnlySet<AgentCapability> Capabilities);

public sealed record ProviderRoutingDecision(
    string ProviderId,
    string AgentId,
    string Reason,
    DateTimeOffset DecidedAt,
    IReadOnlyDictionary<string, string> Evidence);

public enum ProviderKind
{
    OpenAi,
    GitHubModels,
    Ollama,
    DeterministicTest
}
