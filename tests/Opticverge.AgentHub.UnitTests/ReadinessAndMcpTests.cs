using Opticverge.AgentHub.Application.Readiness;
using Opticverge.AgentHub.Domain.Readiness;
using Opticverge.AgentHub.Domain.Security;
using Opticverge.AgentHub.Infrastructure.Mcp;

namespace Opticverge.AgentHub.UnitTests;

public sealed class ReadinessAndMcpTests
{
    [Fact]
    public void Readiness_summary_is_decomposed_into_evidence_checks()
    {
        var summary = new ReadinessService(TimeProvider.System).SummarizeRepository("repo");

        Assert.InRange(summary.Score, 1, 100);
        Assert.All(summary.Checks, check => Assert.False(string.IsNullOrWhiteSpace(check.Evidence)));
        Assert.Contains(summary.Checks, check => check.Status is ReadinessCheckStatus.Warning);
    }

    [Fact]
    public void Privileged_mcp_tools_require_elevated_policy()
    {
        var replay = new McpToolCatalog().GetTool("replay_agent_run");

        Assert.NotNull(replay);
        Assert.True(replay.IsPrivileged);
        Assert.Equal(AgentHubPolicies.ReplayEvents, replay.RequiredPolicy);
    }
}
