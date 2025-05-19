using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure;
using Dapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers.Add(new OpenApiServer
        {
            Url = "https://localhost:5001"
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks().AddCheck<CustomSqlHealthCheck>("custom-sql");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.Purple);
    });

    app.ApplyMigrations();

    app.SeedData();
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public class CustomSqlHealthCheck(ISqlConnectionFactory sqlConnectionFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = sqlConnectionFactory.CreateConnection();

            await connection.ExecuteScalarAsync("SELECT 1;");

            return HealthCheckResult.Healthy();
        }
        catch (Exception e) 
        {
            return HealthCheckResult.Unhealthy(exception: e);
        }
    }
}