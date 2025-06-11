using System.Reflection;
using Amazon.S3;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Octokit;
using Octokit.Internal;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using StackExchange.Redis;
using Texnokaktus.ProgOlymp.ContentService.DataAccess;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Endpoints;
using Texnokaktus.ProgOlymp.ContentService.Handlers;
using Texnokaktus.ProgOlymp.ContentService.Services;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;
using Texnokaktus.ProgOlymp.OpenTelemetry;
using YandexContestClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb"))
                                                               .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services
       .AddScoped<IContentResolver<GitHubReleaseItem>, GitHubReleaseContentResolver>()
       .AddScoped<IContentResolver<S3Item>, S3ContentResolver>()
       .AddScoped<IContentResolver<YandexContestProblemTestItem>, YandexContestProblemTestResolver>()
       .AddScoped<IContentResolverFactory, ContentResolverFactory>()
       .AddScoped<IContentItemQueryHandler, ContentItemQueryHandler>()
       .AddScoped<IContentLinkGenerator, ContentLinkGenerator>()
       .AddScoped<IContestContentQueryHandler, ContentQueryHandler>()
       .AddScoped<IContestStageContentQueryHandler, ContentQueryHandler>()
       .AddScoped<IContestProblemContentQueryHandler, ContentQueryHandler>();

var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("DefaultRedis")!);
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

builder.Services
       .AddDataProtection(options => options.ApplicationDiscriminator = Assembly.GetEntryAssembly()?.GetName().Name)
       .PersistKeysToStackExchangeRedis(connectionMultiplexer);

builder.Services.AddHttpContextAccessor();

builder.Services
       .AddYandexContestClient()
       .AddYandexContestAuthentication<TokenProvider>();

builder.Services
       .AddScoped<ICredentialStore>(_ =>
        {
            var accessToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
                           ?? throw new InvalidOperationException("GITHUB_TOKEN is not set");

            var credentials = new Credentials(accessToken);
            return new InMemoryCredentialStore(credentials);
        })
       .AddScoped<GitHubClient>(provider => ActivatorUtilities.CreateInstance<GitHubClient>(provider, new ProductHeaderValue("ContentService")));

var awsOptions = builder.Configuration.GetAWSOptions("S3");
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
                                                                 .AddOpenTelemetrySupport(builder.Configuration));

builder.Services.AddTexnokaktusOpenTelemetry(builder.Configuration,
                                             "ContentService",
                                             tracerProviderBuilder => tracerProviderBuilder.AddAWSInstrumentation(),
                                             meterProviderBuilder => meterProviderBuilder.AddAWSInstrumentation());

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.MapGroup("/").WithTags("Content retrieval").MapContentEndpoints();
app.MapGroup("api").WithTags("Content navigation").MapContentApiEndpoints();

await app.RunAsync();
