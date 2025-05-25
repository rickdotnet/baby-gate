using AspNet.Security.OAuth.GitHub;
using AuthServer.Components;
using AuthServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace AuthServer;

public static class Startup
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var config = builder.AddConfig();
        builder.Services
            .AddHttpContextAccessor()
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddScoped<JwtService>();
    
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGitHub(options =>
            {
                options.CallbackPath = new PathString("/auth/callback-gh");
                options.ClientId = config.GitHubClient;
                options.ClientSecret = config.GitHubSecret;
            });
        
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        
        return app;
    }
    
    
    private static AuthServerConfig AddConfig(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables("BABY_");
        builder.Configuration.AddJsonFile("authServer.json", optional: true);
        builder.Services.Configure<AuthServerConfig>(builder.Configuration);
        builder.Services.AddTransient<AuthServerConfig>(x => x.GetRequiredService<IOptions<AuthServerConfig>>().Value);

        return builder.Configuration.Get<AuthServerConfig>()!;
    }
}
