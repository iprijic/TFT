using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using TFT.API.Business.Model;
using TFT.API.Rest;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OData;
using Microsoft.AspNetCore.Http;
using TFT.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();

//builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(builder.Environment.ContentRootPath, "DataSource"));

RepositoryBuilder.Build(builder);
//builder.Services.AddSingleton<RepositoryBuilder>();

builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents()
        {
            OnAuthenticationFailed = context =>
            {
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


builder.Services.AddDbContext<Entities>(options => options
    .UseSqlServer(builder.Configuration["Entities:target"]))
    .AddControllers()
    .AddOData(opt => opt.AddRouteComponents(builder.Configuration["dataschemaPrefix"], ODataBuilder.GetEdmModel())
    .Select()
    .Filter()
    .OrderBy()
    .SetMaxTop(20)
    .Count()
    .Expand());


var app = builder.Build();

//app.Services.GetService<RepositoryBuilder>();

// Configure the HTTP request pipeline.

//app.UsePathBase(builder.Configuration["base"]);
//app.Use((context, next) =>
//{
//    context.Request.PathBase = builder.Configuration["base"];
//    return next();
//});

app.Use((context, next) =>
{
    if (Boolean.Parse(builder.Configuration["Debugging:DisableAuth"]) == false && context.User.Identity.IsAuthenticated == false)
    {
        if(context.Request.Method.ToUpper() == "POST" && context.Request.Path.StartsWithSegments(new PathString("/authentication")))
        {
            return next();
        }

        context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

        ODataError error = new ODataError()
        {
            ErrorCode = context.Response.StatusCode.ToString(),
            Message = "Unauthorized access",
            Target = "API"
        };

        context.Response.WriteAsync(JsonSerializer.Serialize<ODataError>(error));
        return Task.CompletedTask;
    }

    return next();
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.MapControllerRoute(name: "authentication1",
//                pattern: "authentication/{*SignIn}",
//                defaults: new { controller = "Authentication" /*, action = "SignIn"*/ });

//app.MapControllerRoute(name: "authentication2",
//                pattern: "authentication/{*SignOutApi}",
//                defaults: new { controller = "Authentication" /*, action = "SignOutApi" */});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/");

app.Run();
