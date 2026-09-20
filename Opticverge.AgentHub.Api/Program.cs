using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Opticverge.AgentHub.Api.Hubs;
using Opticverge.AgentHub.Application;
using Opticverge.AgentHub.Application.Agents;
using Opticverge.AgentHub.Application.Providers;
using Opticverge.AgentHub.Application.Readiness;
using Opticverge.AgentHub.Application.Telemetry;
using Opticverge.AgentHub.Domain.Security;
using Opticverge.AgentHub.Evaluation;
using Opticverge.AgentHub.Evaluation.Datasets;
using Opticverge.AgentHub.Evaluation.Evaluators;
using Opticverge.AgentHub.Infrastructure;
using Opticverge.AgentHub.Infrastructure.Caching;
using Opticverge.AgentHub.Infrastructure.Mcp;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddAgentHubApplication();
builder.Services.AddAgentHubInfrastructure();
builder.Services.AddAgentHubEvaluation();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"] ?? "http://localhost:8080/realms/agenthub";
        options.Audience = builder.Configuration["Authentication:Audience"] ?? "agenthub-api";
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AgentHubPolicies.RunAgents, policy => policy.RequireClaim("agenthub_role", "admin", "platform-engineer"))
    .AddPolicy(AgentHubPolicies.ViewCostData, policy => policy.RequireClaim("agenthub_role", "admin", "platform-engineer", "viewer"))
    .AddPolicy(AgentHubPolicies.ReplayEvents, policy => policy.RequireClaim("agenthub_role", "admin", "platform-engineer"))
    .AddPolicy(AgentHubPolicies.ManageProviderSettings, policy => policy.RequireClaim("agenthub_role", "admin"))
    .AddPolicy(AgentHubPolicies.InvokePrivilegedMcpTools, policy => policy.RequireClaim("agenthub_role", "admin", "platform-engineer"));

var signalR = builder.Services.AddSignalR();
var redisConnection = builder.Configuration.GetConnectionString(CachePlan.RedisResourceName);
if (!string.IsNullOrWhiteSpace(redisConnection))
{
    signalR.AddStackExchangeRedis(redisConnection, options =>
    {
        options.Configuration.ChannelPrefix = RedisChannel.Literal(CachePlan.SignalRChannelPrefix);
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    service = "Opticverge.AgentHub.Api",
    capabilities = new[]
    {
        "agent registry",
        "provider routing",
        "evaluation baseline checks",
        "readiness evidence",
        "SignalR agent run updates"
    }
}));

app.MapGet("/api/agents", (IAgentRegistry registry) => registry.ListAgents())
    .RequireAuthorization(AgentHubPolicies.RunAgents)
    .WithName("ListAgents");

app.MapGet("/api/agents/{agentId}", (string agentId, IAgentRegistry registry) =>
    registry.GetAgent(agentId) is { } agent ? Results.Ok(agent) : Results.NotFound())
    .RequireAuthorization(AgentHubPolicies.RunAgents)
    .WithName("GetAgent");

app.MapPost("/api/agent-runs", async (
    AgentRunRequest request,
    AgentRunService runs,
    AgentHubMetrics metrics,
    IHubContext<AgentRunsHub> hub,
    CancellationToken cancellationToken) =>
{
    var result = runs.RequestRun(request.AgentId, request.RequestedBy);
    if (!result.Accepted || result.Run is null)
    {
        return Results.BadRequest(new { result.Error });
    }

    metrics.AgentRunsStarted.Add(1, KeyValuePair.Create<string, object?>("agent.id", result.Run.AgentId));
    metrics.ProviderRoutingDecisions.Add(1, KeyValuePair.Create<string, object?>("provider.id", result.Run.ProviderId));

    await hub.Clients.Group("agent-runs").SendAsync("agentRunRequested", result.Run, cancellationToken);
    return Results.Accepted($"/api/agent-runs/{result.Run.RunId}", result);
})
    .RequireAuthorization(AgentHubPolicies.RunAgents)
    .WithName("RequestAgentRun");

app.MapGet("/api/providers", (IProviderCatalog providers) => providers.ListProviders())
    .RequireAuthorization(AgentHubPolicies.ViewCostData)
    .WithName("ListProviders");

app.MapGet("/api/readiness", (ReadinessService readiness) => readiness.SummarizeRepository("Opticverge.AgentHub"))
    .RequireAuthorization(AgentHubPolicies.RunAgents)
    .WithName("GetReadinessSummary");

app.MapPost("/api/evaluations/deterministic", (
    EvaluationRequest request,
    EvaluationRunner runner,
    BaselineComparer comparer,
    AgentHubMetrics metrics) =>
{
    var result = runner.Run(request.Dataset, request.Candidates);
    var comparison = comparer.Compare(request.Baseline, result);
    metrics.EvaluationRuns.Add(1, KeyValuePair.Create<string, object?>("dataset.id", request.Dataset.Id));
    return Results.Ok(new { result, comparison });
})
    .RequireAuthorization(AgentHubPolicies.ViewCostData)
    .WithName("RunDeterministicEvaluation");

app.MapGet("/api/mcp/tools", (McpToolCatalog catalog) => catalog.ListTools())
    .RequireAuthorization(AgentHubPolicies.InvokePrivilegedMcpTools)
    .WithName("ListMcpTools");

app.MapHub<AgentRunsHub>("/hubs/agent-runs")
    .RequireAuthorization(AgentHubPolicies.RunAgents);

app.Run();

public sealed record AgentRunRequest(string AgentId, string RequestedBy);

public sealed record EvaluationRequest(
    EvaluationDataset Dataset,
    IReadOnlyList<EvaluationCandidate> Candidates,
    EvaluationBaseline Baseline);
