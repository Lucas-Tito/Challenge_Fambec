using Microsoft.EntityFrameworkCore;
using Challenge_Fambec.Shared.Models.Entities;
using Challenge_Fambec.Shared.Models;

namespace Challenge_Fambec.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Unique constraint on CodItem
                entity.HasIndex(e => e.CodItem).IsUnique();
                
                // Required fields configuration
                entity.Property(e => e.CodItem).HasMaxLength(60).IsRequired();
                entity.Property(e => e.DescrItem).IsRequired();
                entity.Property(e => e.UnidInv).HasMaxLength(6).IsRequired();
                entity.Property(e => e.TipoItem).IsRequired();
                
                // Optional fields with specific lengths
                entity.Property(e => e.CodAntItem).HasMaxLength(60);
                entity.Property(e => e.CodNcm).HasMaxLength(8);
                entity.Property(e => e.ExIpi).HasMaxLength(3);
                entity.Property(e => e.CodGen).HasMaxLength(2);
                entity.Property(e => e.CodLst).HasMaxLength(5);
                
                // Decimal precision for ICMS rate
                entity.Property(e => e.AliqIcms).HasPrecision(5, 2);
                
                // Timestamp configuration
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.DataAtualizacao).HasDefaultValueSql("GETUTCDATE()");
                
                // Index for better performance on searches
                entity.HasIndex(e => e.CodItem);
                entity.HasIndex(e => e.TipoItem);
            });
        }
    }
}