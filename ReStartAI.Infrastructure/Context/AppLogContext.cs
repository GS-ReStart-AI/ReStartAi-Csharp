using Microsoft.EntityFrameworkCore;

namespace ReStartAI.Infrastructure.Context
{
    public class AppLogContext : DbContext
    {
        public DbSet<LogEntry> Logs { get; set; }

        public AppLogContext(DbContextOptions<AppLogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("Logs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }

    public class LogEntry
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}