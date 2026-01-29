using System.Reflection;
using Microsoft.OpenApi.Models;
using TennisStats.Application;
using TennisStats.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file in Development
if (builder.Environment.IsDevelopment())
{
    var envPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
    if (File.Exists(envPath))
    {
        foreach (var line in File.ReadAllLines(envPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex > 0)
            {
                var key = line[..separatorIndex].Trim();
                var value = line[(separatorIndex + 1)..].Trim();
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}

// Add configuration from environment variables with TENNISSTATS_ prefix
builder.Configuration.AddEnvironmentVariables("TENNISSTATS_");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tennis Statistics API",
        Version = "v1",
        Description = "API for tennis statistics application supporting WTA and ATP data. " +
                      "Use the Import endpoints to populate the database with historical data from the external API.",
        Contact = new OpenApiContact
        {
            Name = "Tennis Stats Team"
        },
        License = new OpenApiLicense
        {
            Name = "MIT"
        }
    });

    // Include XML comments from controllers
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add tags for grouping endpoints
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
            return new[] { api.GroupName };

        if (api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller))
            return new[] { controller };

        return new[] { "Other" };
    });

    c.DocInclusionPredicate((name, api) => true);
});

// Add Application and Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add CORS for Vue.js frontend
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',') 
    ?? new[] { "http://localhost:5173", "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tennis Statistics API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Tennis Statistics API";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
