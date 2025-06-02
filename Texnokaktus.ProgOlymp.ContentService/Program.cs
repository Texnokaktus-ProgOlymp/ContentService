using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Octokit;
using Octokit.Internal;
using Texnokaktus.ProgOlymp.ContentService.DataAccess;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Context;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Services;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb"))
                                                               .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services
       .AddScoped<IContentResolver<GitHubReleaseItem>, GitHubReleaseContentResolver>()
       .AddScoped<IContentResolver<S3Item>, S3ContentResolver>()
       .AddScoped<IContentResolverFactory, ContentResolverFactory>();

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

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.RunAsync();
