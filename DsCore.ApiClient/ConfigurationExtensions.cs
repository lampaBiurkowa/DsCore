using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DsCore.ApiClient;

public static class ConfigurationExtensions
{
    public static void AddDsCore(this IConfiguration configuration, IServiceCollection services)
    {
        services.AddOptions<DsCoreOptions>()
            .Bind(configuration.GetSection(DsCoreOptions.SECTION));

        services.AddHttpClient();
        services.AddTransient<DsCoreClientFactory>();
        
        var tokenOptions = configuration.GetSection(DsCoreOptions.SECTION).Get<DsCoreOptions>() ?? throw new("No auth options");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidIssuer = tokenOptions.Url,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecretKey))
                };
            });
    }
}
