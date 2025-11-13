using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using MongoDB.Driver;
using ReStartAI.Infrastructure.Context;
using ReStartAI.Infrastructure.Repositories;
using ReStartAI.Application.Services;
using ReStartAI.Application.Security;
using ReStartAI.Application.IoT;
using ReStartAI.Application.WhyMe;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/restartai-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ReStartAI.Api"))
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();
        t.AddHttpClientInstrumentation();
        t.AddEntityFrameworkCoreInstrumentation();
        t.AddConsoleExporter();
    })
    .WithMetrics(m =>
    {
        m.AddRuntimeInstrumentation();
        m.AddAspNetCoreInstrumentation();
        m.AddHttpClientInstrumentation();
        m.AddConsoleExporter();
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddHealthChecks().AddMongoDb(
    clientFactory: _ => new MongoClient(builder.Configuration["MongoSettings:ConnectionString"]!),
    databaseNameFactory: _ => builder.Configuration["MongoSettings:DatabaseName"]!,
    name: "mongodb"
);

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddDbContext<AppLogContext>(options =>
    options.UseSqlite("Data Source=logs.db"));
builder.Services.AddScoped<LogRepository>();

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddScoped<InsightTriggerService>();
builder.Services.AddHttpClient<InsightClient>();
builder.Services.AddHttpClient<WhyMeGenerator>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
