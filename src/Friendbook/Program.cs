using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Friendbook.BusinessLayer;
using Friendbook.BusinessLayer.Profiles;
using Friendbook.BusinessLayer.Resources;
using Friendbook.DataAccessLayer;
using Friendbook.Validations;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using OperationResults.AspNetCore;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);
builder.AddJsonConfigurationFile("appsettings.local.json");

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
});

builder.Services.AddApplicationServices();
builder.Services.AddApplicationDbContext(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("SqlConnection");
});

builder.Services.AddAutoMapper(typeof(PersonProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
})
.AddValidatorsFromAssemblyContaining<SavePersonRequestValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddDateOnlyTimeOnly();
    options.AddDefaultResponse();
    options.AddAcceptLanguageHeader();
})
.AddFluentValidationRulesToSwagger(options =>
{
    options.SetNotNullableIfMinLengthGreaterThenZero = true;
});

builder.Services.AddRequestLocalization("en", "it");

builder.Services.AddOperationResult((state) => Messages.ValidationErrors);
builder.Services.AddProblemDetails();

var app = builder.Build();
await ConfigureDatabaseAsync(app.Services);

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    // Error handling
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        AllowStatusCode404Response = true,
        ExceptionHandler = async (HttpContext context) =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var error = exceptionHandlerFeature?.Error;

            if (context.RequestServices.GetRequiredService<IProblemDetailsService>() is { } problemDetailsService)
            {
                // Write as JSON problem details
                await problemDetailsService.WriteAsync(new()
                {
                    HttpContext = context,
                    AdditionalMetadata = exceptionHandlerFeature?.Endpoint?.Metadata,
                    ProblemDetails =
                    {
                        Status = context.Response.StatusCode,
                        Title = error?.GetType().FullName ?? "an error occurred while processing your request",
                        Detail = error?.Message
                    }
                });
            }
            else
            {
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                var message = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode) switch
                {
                    { Length: > 0 } reasonPhrase => reasonPhrase,
                    _ => "An error occurred"
                };
                await context.Response.WriteAsync(message + "\r\n");
                await context.Response.WriteAsync($"Request ID: {Activity.Current?.Id ?? context.TraceIdentifier}");
            }
        }
    });
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Friendbook API v1");
});

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task ConfigureDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<IDbContext>();
    await db.MigrateAsync();
}