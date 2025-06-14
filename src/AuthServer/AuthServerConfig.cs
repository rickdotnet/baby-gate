namespace AuthServer;

public record AuthServerConfig
{
    public string NatsUrl { get; set; } = "nats://localhost:4222";
    public string NatsUser { get; set; } = string.Empty;
    public string NatsPassword { get; set; } = string.Empty;
    public string NatsSeed { get; set; } = string.Empty;
    public string GitHubClient { get; set; } = string.Empty;
    public string GitHubSecret { get; set; } = string.Empty;
}
