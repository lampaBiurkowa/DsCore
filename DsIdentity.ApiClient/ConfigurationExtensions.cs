using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DsIdentity.ApiClient;

public static class ConfigurationExtensions
{
    public static void AddDsIdentity(this IConfiguration configuration, IServiceCollection services)
    {
        services.AddOptions<DsIdentityOptions>()
            .Bind(configuration.GetSection(DsIdentityOptions.SECTION));

        services.AddHttpClient();
        services.AddTransient<DsIdentityClientFactory>();
        
        var tokenOptions = configuration.GetSection(DsIdentityOptions.SECTION).Get<DsIdentityOptions>() ?? throw new("No auth options");
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
