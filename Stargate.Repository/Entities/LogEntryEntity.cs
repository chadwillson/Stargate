using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stargate.Repository.Entities
{

    [Table("LogEntry")]
    public class LogEntryEntity
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Level { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Exception { get; set; }

        public string? StackTrace { get; set; }

        public string? Source { get; set; }

        public string? CorrelationId { get; set; }

        public string? UserId { get; set; }

        public string? RequestPath { get; set; }

        public string? RequestMethod { get; set; }

        public int? StatusCode { get; set; }

        public long? ElapsedMilliseconds { get; set; }

        public string? AdditionalData { get; set; }

    }

    public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntryEntity>
    {
        public void Configure(EntityTypeBuilder<LogEntryEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Timestamp).IsRequired();
            builder.Property(x => x.Level).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Category).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Message).IsRequired();
            builder.Property(x => x.Exception).HasMaxLength(4000);
            builder.Property(x => x.StackTrace).HasMaxLength(4000);
            builder.Property(x => x.Source).HasMaxLength(255);
            builder.Property(x => x.CorrelationId).HasMaxLength(50);
            builder.Property(x => x.UserId).HasMaxLength(100);
            builder.Property(x => x.RequestPath).HasMaxLength(500);
            builder.Property(x => x.RequestMethod).HasMaxLength(10);
            builder.Property(x => x.AdditionalData).HasMaxLength(4000);

            builder.HasIndex(x => x.Timestamp);
            builder.HasIndex(x => x.Level);
            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => x.CorrelationId);
        }
    }

}
