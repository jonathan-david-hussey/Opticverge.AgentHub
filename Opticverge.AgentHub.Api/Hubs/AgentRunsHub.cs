using Microsoft.AspNetCore.SignalR;

namespace Opticverge.AgentHub.Api.Hubs;

public sealed class AgentRunsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "agent-runs", Context.ConnectionAborted);
        await base.OnConnectedAsync();
    }
}
