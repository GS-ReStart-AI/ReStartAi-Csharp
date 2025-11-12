using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReStart.AI API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var cfg = builder.Configuration;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg["Jwt:Issuer"],
            ValidAudience = cfg["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddHealthChecks().AddMongoDb(
    sp => new MongoClient(cfg["Mongo:ConnectionString"]),
    sp => cfg["Mongo:Database"],
    name: "mongodb"
);

var app = builder.Build();

app.UseSwagger();

var swaggerKeyHeader = cfg["Swagger:ApiKeyHeaderName"] ?? "x-api-key";
var swaggerKeyValue  = cfg["Swagger:ApiKey"] ?? string.Empty;

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/swagger"))
    {
        if (!ctx.Request.Headers.TryGetValue(swaggerKeyHeader, out var provided) ||
            string.IsNullOrEmpty(swaggerKeyValue) ||
            !string.Equals(provided.ToString(), swaggerKeyValue, StringComparison.Ordinal))
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsync("Swagger UI requires valid API Key.");
            return;
        }
    }
    await next();
});

app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
