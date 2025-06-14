using NATS.Jwt;
using NATS.Jwt.Models;
using NATS.NKeys;

namespace AuthServer.Services
{
    public class JwtUtility
    {
        private readonly KeyPair keyPair;
        private readonly NatsJwt natsJwt = new();

        public JwtUtility(AuthServerConfig authServerConfig)
        {
            keyPair = KeyPair.FromSeed(authServerConfig.NatsSeed);
        }

        public string CreateJwtForUserAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return string.Empty;

            var user = natsJwt.NewUserClaims(userName);

            user.Name = user.Subject;
            user.Issuer = keyPair.GetPublicKey();
            user.Audience = "APP"; // required
            user.Expires = DateTimeOffset.UtcNow.AddDays(30);
            user.IssuedAt = DateTimeOffset.UtcNow;

            user.User = new()
            {
                Pub = new NatsPermission
                {
                    Allow = [userName, $"{userName}.>"],
                    Deny = []
                },
                Sub = new NatsPermission
                {
                    Allow = [">"],
                    Deny = []
                }
            };

            var userJwt = natsJwt.EncodeUserClaims(user, keyPair);

            return userJwt;
        }
    }
}
