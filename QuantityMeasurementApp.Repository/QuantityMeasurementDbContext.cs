using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
      public class QuantityMeasurementDbContext : DbContext
      {
            // Constructor used by Dependency Injection
            public QuantityMeasurementDbContext(
                DbContextOptions<QuantityMeasurementDbContext> options)
                : base(options)
            {
            }

            // ── DbSets (Tables) ───────────────────────────────────

            public DbSet<QuantityMeasurementEntity> Measurements { get; set; }
            public DbSet<UserEntity> Users { get; set; }

            // Configure database if not already configured (for migrations / fallback)
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                  if (!optionsBuilder.IsConfigured)
                  {
                        optionsBuilder.UseSqlServer(
             "Server=localhost\\SQLEXPRESS;" +
             "Database=QuantityMeasurementDB;" +
             "Trusted_Connection=True;" +
             "TrustServerCertificate=True;");
                  }
            }

            // Model configuration (Fluent API)
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  // ── QuantityMeasurementEntity ─────────────────────

                  modelBuilder.Entity<QuantityMeasurementEntity>(entity =>
                  {
                        entity.HasKey(e => e.Id);

                        entity.Property(e => e.Operand1Value)
                        .IsRequired();

                        entity.Property(e => e.Operand1Unit)
                        .IsRequired()
                        .HasMaxLength(50);

                        entity.Property(e => e.Operand1MeasurementType)
                        .IsRequired()
                        .HasMaxLength(50);

                        entity.Property(e => e.Operand2Value)
                        .IsRequired(false);

                        entity.Property(e => e.Operand2Unit)
                        .IsRequired(false)
                        .HasMaxLength(50);

                        entity.Property(e => e.Operand2MeasurementType)
                        .IsRequired(false)
                        .HasMaxLength(50);

                        entity.Property(e => e.Operation)
                        .IsRequired()
                        .HasMaxLength(20);

                        entity.Property(e => e.ResultValue)
                        .IsRequired(false);

                        entity.Property(e => e.ResultUnit)
                        .IsRequired(false)
                        .HasMaxLength(50);

                        entity.Property(e => e.ResultMeasurementType)
                        .IsRequired(false)
                        .HasMaxLength(50);

                        entity.Property(e => e.ComparisonResult)
                        .IsRequired()
                        .HasDefaultValue(false);

                        entity.Property(e => e.ScalarResult)
                        .IsRequired()
                        .HasDefaultValue(0.0);

                        entity.Property(e => e.HasError)
                        .IsRequired()
                        .HasDefaultValue(false);

                        entity.Property(e => e.ErrorMessage)
                        .IsRequired(false)
                        .HasMaxLength(500);

                        entity.Property(e => e.Timestamp)
                        .IsRequired();

                        // Indexes for fast queries
                        entity.HasIndex(e => e.Operation)
                        .HasDatabaseName("IX_Measurements_Operation");

                        entity.HasIndex(e => e.Operand1MeasurementType)
                        .HasDatabaseName("IX_Measurements_MeasurementType");

                        entity.HasIndex(e => e.Timestamp)
                        .HasDatabaseName("IX_Measurements_Timestamp");
                  });

                  // ── UserEntity ────────────────────────────────────

                  modelBuilder.Entity<UserEntity>(entity =>
                  {
                        entity.HasKey(e => e.Id);

                        entity.Property(e => e.FullName)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(e => e.Email)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(e => e.PasswordHash)
                        .IsRequired();

                        entity.Property(e => e.Role)
                        .HasConversion<string>()
                        .HasMaxLength(20)
                        .IsRequired();

                        entity.Property(e => e.CreatedAt)
                        .IsRequired();

                        entity.Property(e => e.IsActive)
                        .IsRequired()
                        .HasDefaultValue(true);

                        // Unique index on Email — no duplicate accounts
                        entity.HasIndex(e => e.Email)
                        .IsUnique()
                        .HasDatabaseName("IX_Users_Email");
                  });
            }
      }
}