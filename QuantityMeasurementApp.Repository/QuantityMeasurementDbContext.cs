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

        // DbSet (Table)
        public DbSet<QuantityMeasurementEntity> Measurements { get; set; }

        // Configure database if not already configured (for migrations / fallback)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\MSSQLLocalDB;" +
                    "Database=QuantityMeasurementDB;" +
                    "Trusted_Connection=True;" +
                    "TrustServerCertificate=True;");
            }
        }

        // Model configuration (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuantityMeasurementEntity>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Required Fields
                entity.Property(e => e.Timestamp)
                      .IsRequired();

                entity.Property(e => e.Operation)
                      .HasMaxLength(20)
                      .IsRequired();

                // Optional Fields with length constraints
                entity.Property(e => e.Operand1Unit)
                      .HasMaxLength(50);

                entity.Property(e => e.Operand1MeasurementType)
                      .HasMaxLength(50);

                entity.Property(e => e.Operand2Unit)
                      .HasMaxLength(50);

                entity.Property(e => e.Operand2MeasurementType)
                      .HasMaxLength(50);

                entity.Property(e => e.ResultUnit)
                      .HasMaxLength(50);

                entity.Property(e => e.ResultMeasurementType)
                      .HasMaxLength(50);

                entity.Property(e => e.ErrorMessage)
                      .HasMaxLength(500);

                // Indexes (for performance)
                entity.HasIndex(e => e.Operation)
                      .HasDatabaseName("IX_Measurements_Operation");

                entity.HasIndex(e => e.Operand1MeasurementType)
                      .HasDatabaseName("IX_Measurements_MeasurementType");

                entity.HasIndex(e => e.Timestamp)
                      .HasDatabaseName("IX_Measurements_Timestamp");
            });
        }
    }
}