using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
using ReStartAI.Domain.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using ReStartAI.Api.Swagger.Examples.Usuarios;
using ReStartAI.Application.Pdf;
using ReStartAI.Api.Integration;



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

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ReStart.AI API",
        Version = "v1",
        Description = "API versionada do projeto ReStart.AI - IoT + IA Generativa"
    });

    options.EnableAnnotations();
    options.ExampleFilters();

    var headerName = builder.Configuration["Swagger:ApiKeyHeaderName"] ?? "x-api-key";

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = $"Informe a API Key no header `{headerName}`",
        Name = headerName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpClient<IResumeSummaryClient, ResumeSummaryClient>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<UsuarioCreateRequestExample>();
builder.Services.Configure<IotOptions>(builder.Configuration.GetSection("Iot"));
builder.Services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();
builder.Services.AddScoped<IAppEventRepository, AppEventRepository>();
builder.Services.AddScoped<ICurriculoRepository, CurriculoRepository>();
builder.Services.AddScoped<IVagaRepository, VagaRepository>();
builder.Services.AddScoped<ICandidaturaRepository, CandidaturaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<INotificacaoRepository, NotificacaoRepository>();

builder.Services.AddScoped<AppEventService>();
builder.Services.AddScoped<CurriculoService>();
builder.Services.AddScoped<VagaService>();
builder.Services.AddScoped<CandidaturaService>();
builder.Services.AddScoped<NotificacaoService>();
builder.Services.AddScoped<UsuarioService>();

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
builder.Services.AddScoped<InsightTriggerService>();
builder.Services.AddHttpClient<InsightClient>();
builder.Services.AddHttpClient<WhyMeGenerator>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ReStart.AI API v1");
    options.RoutePrefix = "swagger";
});

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
