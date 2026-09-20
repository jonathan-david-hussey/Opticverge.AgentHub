using Microsoft.Extensions.DependencyInjection;
using Opticverge.AgentHub.Application.Providers;
using Opticverge.AgentHub.Infrastructure.Agents;
using Opticverge.AgentHub.Infrastructure.Mcp;

namespace Opticverge.AgentHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAgentHubInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IProviderCatalog, InMemoryProviderCatalog>();
        services.AddSingleton<McpToolCatalog>();
        return services;
    }
}
