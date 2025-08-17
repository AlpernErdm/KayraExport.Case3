using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace LogService.Infrastructure.Data;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {
    }

    public DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<LogEntry>(entity =>
        {
            entity.ToTable("LogEntries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Level).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.ContextData);
            entity.Property(e => e.Exception);
            entity.Property(e => e.UserId);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.IsActive).IsRequired();
            
            entity.HasIndex(e => e.ServiceName);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.CorrelationId);
            entity.HasIndex(e => e.UserId);
        });
    }
}
