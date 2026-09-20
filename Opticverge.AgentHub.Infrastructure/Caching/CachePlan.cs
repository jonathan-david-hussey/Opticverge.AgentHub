namespace Opticverge.AgentHub.Infrastructure.Caching;

public static class CachePlan
{
    public const string KeyPrefix = "agenthub";
    public const string RedisResourceName = "cache";
    public const string SignalRChannelPrefix = "agenthub";

    public static string BuildKey(string area, string id) => $"{KeyPrefix}:{area}:{id}";
}
