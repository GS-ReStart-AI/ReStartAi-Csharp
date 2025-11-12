using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using MongoDB.Driver;
using ReStartAI.Infrastructure.Context;
using ReStartAI.Infrastructure.Repositories;
using ReStartAI.Application.Security;
using ReStartAI.Application.IoT;
using ReStartAI.Application.WhyMe;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ReStart.AI API",
        Version = "v1",
        Description = "API do projeto ReStart.AI - IoT + IA Generativa"
    });

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
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
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
    clientFactory: _ => new MongoClient(cfg["MongoSettings:ConnectionString"]!),
    databaseNameFactory: _ => cfg["MongoSettings:DatabaseName"]!,
    name: "mongodb");

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVagaRepository, VagaRepository>();
builder.Services.AddScoped<ICurriculoRepository, CurriculoRepository>();
builder.Services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
builder.Services.AddScoped<ICandidaturaRepository, CandidaturaRepository>();
builder.Services.AddScoped<IAppEventRepository, AppEventRepository>();

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddScoped<InsightTriggerService>();
builder.Services.AddHttpClient<InsightClient>();
builder.Services.AddHttpClient<WhyMeGenerator>();
builder.Services.AddDbContext<AppLogContext>(options =>
    options.UseSqlite("Data Source=logs.db"));
builder.Services.AddScoped<LogRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReStart.AI v1");
    c.RoutePrefix = "swagger";
});

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
