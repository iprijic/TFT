using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData;
using System.Text;
using System.Text.Json;

namespace TFT.API.Security
{
    public static class AuthenticationServiceExtensions
    {
        public static AuthenticationBuilder AddJWTAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
            return services.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
             })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents()
        {
            OnAuthenticationFailed = context =>
            {
                context.NoResult();
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                ODataError error = new ODataError()
                {
                    ErrorCode = context.Response.StatusCode.ToString(),
                    Message = "Unauthorized access",
                    Target = "API"
                };

                if (context.HttpContext.Items.ContainsKey(nameof(JwtBearerEvents.OnAuthenticationFailed)))
                {
                    context.HttpContext.Items.Remove(nameof(JwtBearerEvents.OnAuthenticationFailed));
                }

                context.HttpContext.Items.Add(nameof(JwtBearerEvents.OnAuthenticationFailed), true);

                context.Response.WriteAsync(JsonSerializer.Serialize<ODataError>(error)).Wait();
                return Task.CompletedTask;
            },

            OnForbidden = context =>
            {
                return Task.CompletedTask;
            },

            OnMessageReceived = context =>
            {
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                return Task.CompletedTask;
            }
        };
    });
        }
    }
}
