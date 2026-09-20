using Microsoft.Extensions.DependencyInjection;
using Opticverge.AgentHub.Evaluation.Evaluators;

namespace Opticverge.AgentHub.Evaluation;

public static class DependencyInjection
{
    public static IServiceCollection AddAgentHubEvaluation(this IServiceCollection services)
    {
        services.AddSingleton<IEvaluationScorer, DeterministicEvaluationScorer>();
        services.AddSingleton<EvaluationRunner>();
        services.AddSingleton<BaselineComparer>();
        return services;
    }
}
