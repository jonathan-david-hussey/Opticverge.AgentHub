namespace Opticverge.AgentHub.Domain.Agents;

public sealed record AgentDefinition(
    string Id,
    string DisplayName,
    string Description,
    AgentCapability Capability,
    bool IsExperimental,
    IReadOnlyList<string> RequiredPolicies);

public enum AgentCapability
{
    CodeReview,
    InvoiceAssistance,
    ReadinessScanning,
    ProviderBenchmarking,
    RunReplay
}
