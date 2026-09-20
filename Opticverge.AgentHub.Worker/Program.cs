using Opticverge.AgentHub.Worker;
using Opticverge.AgentHub.Application;
using Opticverge.AgentHub.Evaluation;
using Opticverge.AgentHub.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddAgentHubApplication();
builder.Services.AddAgentHubInfrastructure();
builder.Services.AddAgentHubEvaluation();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
