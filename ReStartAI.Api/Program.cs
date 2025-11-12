using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using ReStartAI.Infrastructure.Data;
using ReStartAI.Infrastructure.Repositories;
using ReStartAI.Application.Security;
using ReStartAI.Application.IoT;
using ReStartAI.Application.WhyMe;
using ReStartAI.Api.Services;
using HealthChecks.MongoDb;
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
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
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

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICurriculoRepository, CurriculoRepository>();
builder.Services.AddScoped<IVagaRepository, VagaRepository>();
builder.Services.AddScoped<ICandidaturaRepository, CandidaturaRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAppEventRepository, AppEventRepository>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddHttpClient<InsightClient>();
builder.Services.AddHttpClient<WhyMeGenerator>();
builder.Services.AddScoped<InsightTriggerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var swaggerKeyHeader = cfg["Swagger:ApiKeyHeaderName"] ?? "x-api-key";
var swaggerKeyValue = cfg["Swagger:ApiKey"] ?? string.Empty;

if (app.Environment.IsProduction())
{
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
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
