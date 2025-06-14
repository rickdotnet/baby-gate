using Microsoft.Extensions.Caching.Distributed;
using NATS.Client.Core;
using NATS.Jwt.Models;
using NATS.Net;
using Synadia.AuthCallout;

namespace AuthServer.Services;

public class AuthCalloutBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;

    public AuthCalloutBackgroundService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var calloutUtility = serviceProvider.GetRequiredService<CalloutUtility>();
        var connection = serviceProvider.GetRequiredService<INatsConnection>();
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();

        var opts = new NatsAuthServiceOpts(Authorizer, ResponseSigner)
        {
            ErrorHandler = (e, ct) =>
            {
                Console.WriteLine($"ERROR: {e}");
                return default;
            },
        };

        await using var service = new NatsAuthService(connection.CreateServicesContext(), opts);

        await service.StartAsync(stoppingToken);
        
        Console.WriteLine("Auth Callout Service started.");
        await Task.Delay(Timeout.Infinite, stoppingToken);

        return;

        async ValueTask<NatsAuthorizerResult> Authorizer(NatsAuthorizationRequest authRequest, CancellationToken cancellationToken)
        {
            var userJwt = authRequest.NatsConnectOptions.Token;
            if (string.IsNullOrEmpty(userJwt))
            {
                var shortToken = authRequest.NatsConnectOptions.Password;
                if (!string.IsNullOrEmpty(shortToken))
                {
                    userJwt = await cache.GetStringAsync(shortToken, cancellationToken) ?? string.Empty;
                }
            }

            var authResult = calloutUtility.GetAuthorizerResult(authRequest.UserNKey, userJwt);
            return authResult;
        }

        ValueTask<string> ResponseSigner(NatsAuthorizationResponseClaims request, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(calloutUtility.ResponseToken(request));
        }
    }
}
