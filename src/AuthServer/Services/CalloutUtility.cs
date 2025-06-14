using NATS.Jwt;
using NATS.Jwt.Models;
using NATS.NKeys;
using Synadia.AuthCallout;

namespace AuthServer.Services;

public class CalloutUtility
{
    private readonly KeyPair signingKey;
    private readonly string publicKey;
    private readonly NatsJwt natsJwt = new();

    public CalloutUtility(AuthServerConfig config)
    {
        signingKey = KeyPair.FromSeed(config.NatsSeed);
        publicKey = signingKey.GetPublicKey();
    }

    public string ResponseToken(NatsAuthorizationResponseClaims responseClaims) 
        => natsJwt.EncodeAuthorizationResponseClaims(responseClaims, signingKey);

    public NatsAuthorizerResult GetAuthorizerResult(string requestNKey, string userJwt)
    {
        if (string.IsNullOrEmpty(userJwt))
            return new NatsAuthorizerResult(string.Empty, 401, "jwt is empty");
        
        var originalClaims = natsJwt.DecodeUserClaims(userJwt);
        var issuer = originalClaims.Issuer;
        var validIssuer = publicKey == issuer;
        
        if (!validIssuer)
            return new NatsAuthorizerResult(string.Empty, 401, "user is not authorized");

        var user = natsJwt.NewUserClaims(requestNKey);
        user.Expires = originalClaims.Expires;
        user.Audience = originalClaims.Audience;
        user.User = originalClaims.User;

        return new NatsAuthorizerResult(natsJwt.EncodeUserClaims(user, signingKey));
    }
}
