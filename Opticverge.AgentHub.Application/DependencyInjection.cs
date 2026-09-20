using Microsoft.Extensions.DependencyInjection;
using Opticverge.AgentHub.Application.Agents;
using Opticverge.AgentHub.Application.Providers;
using Opticverge.AgentHub.Application.Readiness;
using Opticverge.AgentHub.Application.Telemetry;

namespace Opticverge.AgentHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAgentHubApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IAgentRegistry, AgentRegistry>();
        services.AddSingleton<IProviderRouter, ProviderRouter>();
        services.AddSingleton<AgentRunService>();
        services.AddSingleton<ReadinessService>();
        services.AddSingleton<AgentHubMetrics>();
        return services;
    }
}
