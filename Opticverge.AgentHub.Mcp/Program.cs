using Microsoft.AspNetCore.Authentication.JwtBearer;
using Opticverge.AgentHub.Application;
using Opticverge.AgentHub.Application.Telemetry;
using Opticverge.AgentHub.Domain.Security;
using Opticverge.AgentHub.Evaluation;
using Opticverge.AgentHub.Infrastructure;
using Opticverge.AgentHub.Infrastructure.Mcp;

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
    .AddPolicy(AgentHubPolicies.InvokePrivilegedMcpTools, policy => policy.RequireClaim("agenthub_role", "admin", "platform-engineer"));

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    service = "Opticverge.AgentHub.Mcp",
    protocol = "MCP-compatible HTTP facade",
    audit = "Every privileged invocation is designed to carry caller, tool, correlation id, and policy evidence."
}));

app.MapGet("/mcp/tools", (McpToolCatalog catalog) => catalog.ListTools())
    .RequireAuthorization(AgentHubPolicies.RunAgents)
    .WithName("ListMcpTools");

app.MapPost("/mcp/tools/{toolName}/invoke", (
        string toolName,
        McpInvocationRequest request,
        McpToolCatalog catalog,
        AgentHubMetrics metrics) =>
    {
        var tool = catalog.GetTool(toolName);
        if (tool is null) return Results.NotFound(new { error = $"MCP tool '{toolName}' was not found." });

        metrics.McpToolInvocations.Add(1, KeyValuePair.Create<string, object?>("mcp.tool", tool.Name));
        return Results.Accepted($"/mcp/audit/{request.CorrelationId}", new
        {
            tool = tool.Name,
            tool.RequiredPolicy,
            request.CorrelationId,
            request.IdempotencyKey,
            status = "accepted-for-audit"
        });
    })
    .RequireAuthorization(AgentHubPolicies.InvokePrivilegedMcpTools)
    .WithName("InvokeMcpTool");

app.Run();

public sealed record McpInvocationRequest(
    Guid CorrelationId,
    string IdempotencyKey,
    IReadOnlyDictionary<string, string> Arguments);
