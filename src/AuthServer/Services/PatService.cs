using Microsoft.Extensions.Caching.Distributed;

namespace AuthServer.Services;

public class PatService
{
    private readonly JwtUtility jwtUtility;
    private readonly IDistributedCache distributedCache;

    private static readonly DistributedCacheEntryOptions DefaultCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
    };
    public PatService(JwtUtility jwtUtility, IDistributedCache distributedCache)
    {
        this.jwtUtility = jwtUtility;
        this.distributedCache = distributedCache;
    }
    
    public async Task<string> CreatePat(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return string.Empty;

        var pat = jwtUtility.CreateJwtForUserAsync(userName);
        if (string.IsNullOrEmpty(pat))
            return string.Empty;

        var shortToken = Guid.NewGuid().ToString("N");
        await distributedCache.SetStringAsync(shortToken, pat, DefaultCacheOptions);

        return shortToken;
    }
    
}
