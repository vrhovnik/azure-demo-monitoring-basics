// demo is authored by https://www.twilio.com/blog/export-logs-to-azure-monitor-with-opentelemetry-and-dotnet
// and modified to use as an examples for 
using System.Runtime.InteropServices;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Mvc;
using Monitoring.OpenTelemetryExport;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Read Azure Monitor connection string from configuration
var azmConnectionString = builder.Configuration["AzureMonitorConnectionString"];

// Define attributes for your application
var resourceBuilder = ResourceBuilder.CreateDefault()
    // add attributes for the name and version of the service
    .AddService("Monitoring.OpenTelemetry", serviceVersion: "1.0.0")
    // add attributes for the OpenTelemetry SDK version
    .AddTelemetrySdk()
    // add custom attributes
    .AddAttributes(new Dictionary<string, object>
    {
        ["host.name"] = Environment.MachineName,
        ["os.description"] = RuntimeInformation.OSDescription,
        ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant(),
    });

builder.Logging.ClearProviders()
    .AddOpenTelemetry(loggerOptions =>
    {
        loggerOptions
            // define the resource
            .SetResourceBuilder(resourceBuilder)
            // add custom processor
            .AddProcessor(new CustomLogProcessor())
            // send logs to Azure Monitor
            .AddAzureMonitorLogExporter(options => options.ConnectionString = azmConnectionString)
            // send logs to the console using exporter
            .AddConsoleExporter();

        loggerOptions.IncludeFormattedMessage = true;
        loggerOptions.IncludeScopes = true;
        loggerOptions.ParseStateValues = true;
    });

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello caller at {DateLoaded}!", DateTime.Now);
    return Results.Ok($"Loaded at {DateTime.Now}");
});

app.MapPost("/login", (ILogger<Program> logger, [FromBody] LoginData data) =>
{
    logger.LogInformation("User login attempted: Username {Username}, Password {Password}", data.Username,
        data.Password);
    logger.LogWarning("User login failed: Username {Username}", data.Username);
    return Results.Unauthorized();
});

app.Run();

internal record LoginData(string Username, string Password);