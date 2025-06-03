using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.ContentService.DataAccess.Context;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ContentItem> ContentItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContentItem>(builder =>
        {
            builder.HasKey(contentItem => contentItem.Id);
            builder.Property(contentItem => contentItem.Id).ValueGeneratedOnAdd();
            
            builder.HasIndex(contentItem => new
                    {
                        contentItem.ContestName,
                        contentItem.ContestStage,
                        contentItem.ProblemAlias,
                        contentItem.ShortName
                    })
                   .IsUnique();
        });

        modelBuilder.Entity<GitHubReleaseItem>().HasBaseType<ContentItem>();
        modelBuilder.Entity<S3Item>().HasBaseType<ContentItem>();
        modelBuilder.Entity<YandexContestProblemTestItem>().HasBaseType<ContentItem>();
    }
}
