using DevJourney.Infrastructure.Configuration;
using DevJourney.Application.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use native .NET OpenAPI generator and Scalar UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add CORS policy for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDevelopment", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Application services
builder.Services.AddApplication();

// Add controllers and JSON options
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    // Use camelCase JSON, and keep System.Text.Json polymorphic settings in place
    opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = new Dictionary<string, string[]>();
        foreach (var kvp in context.ModelState)
        {
            var key = kvp.Key;
            var errors = kvp.Value.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? "Invalid value." : e.ErrorMessage).Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
            if (errors.Any()) details[key] = errors;
        }

        var result = new BadRequestObjectResult(new
        {
            error = new
            {
                code = "bad_request",
                message = "One or more validation errors occurred.",
                details = details
            }
        });

        return result;
    };
});

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
// Expose OpenAPI in all environments
app.MapOpenApi();

// Enable CORS
app.UseCors("LocalDevelopment");

// Error handling
app.UseMiddleware<DevJourney.Api.Middleware.ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Run();
