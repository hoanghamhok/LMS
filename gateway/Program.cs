using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// CORS cho FE http://localhost:3000
builder.Services.AddCors(o =>
{
    o.AddPolicy("frontend", p => p
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
        //.AllowCredentials());//
});

// (tuỳ) Auth ở gateway: nếu muốn validate signature thì set Jwt__Key trong env
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidAudience = "lms.api",
            ValidateIssuerSigningKey = !string.IsNullOrEmpty(jwtKey),
            IssuerSigningKey = string.IsNullOrEmpty(jwtKey) ? null
                : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

// ---- YARP: dùng kiểu cụ thể Dictionary/List để gán vào các property IReadOnly* (init-only)
var routes = new[]
{
    new RouteConfig
    {
        RouteId = "identity",
        ClusterId = "identity",
        Match = new RouteMatch { Path = "/api/identity/{**catch-all}" },
        Transforms = new IReadOnlyDictionary<string, string>[]
        {
            new Dictionary<string, string> { ["PathRemovePrefix"] = "/api" }
        }
    }
};

var clusters = new[]
{
    new ClusterConfig
    {
        ClusterId = "identity",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            ["d1"] = new DestinationConfig { Address = "http://identity:8081" }
        }
    }
};

builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);
// ---------------------------------------------------------------

var app = builder.Build();

// Áp CORS ngay trên endpoint reverse proxy (preflight OPTIONS cũng qua đây)
app.UseCors("frontend");


app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run("http://0.0.0.0:8080");
