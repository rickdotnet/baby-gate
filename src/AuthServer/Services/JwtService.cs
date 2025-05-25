using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NATS.Jwt;
using NATS.NKeys;

namespace AuthServer.Services
{
    public class JwtService
    {
        private readonly AuthServerConfig authServerConfig;
        private readonly HttpContext context;
        public JwtService(AuthServerConfig authServerConfig, IHttpContextAccessor contextAccessor)
        {
            this.authServerConfig = authServerConfig;
            context = contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(contextAccessor));
        }
        public Task<string> CreateJwtForUserAsync()
        {
            var userName = context.User.Identity?.Name;
            if(string.IsNullOrEmpty(userName))
                return Task.FromResult("unknown");
            
            var keyPair = KeyPair.FromSeed(authServerConfig.NatsSeed);
            
            var jwt = new NatsJwt();
            var user = jwt.NewUserClaims(userName);
            user.Expires = DateTime.UtcNow.AddDays(30);
            var userJwt = jwt.EncodeUserClaims(user, keyPair);
            
            return Task.FromResult(userJwt);
        }
    }
}

