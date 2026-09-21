using Projects;

namespace Opticverge.AgentHub.Tests;

public sealed class AppHostCompositionTests
{
    [Fact]
    public void AppHost_project_reference_is_available_for_aspire_integration_tests()
    {
        var appHostType = typeof(Opticverge_AgentHub_AppHost);

        Assert.Equal("Opticverge.AgentHub.AppHost", appHostType.Assembly.GetName().Name);
    }
}
