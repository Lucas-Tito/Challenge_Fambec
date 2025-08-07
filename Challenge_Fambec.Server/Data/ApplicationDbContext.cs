using Microsoft.EntityFrameworkCore;
using Challenge_Fambec.Shared.Models.Entities;
using Challenge_Fambec.Shared.Models;
using Challenge_Fambec.Shared.Models.Enums;

namespace Challenge_Fambec.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

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

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Unique constraint on Firebase UID
                entity.HasIndex(e => e.FirebaseUid).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                // Required fields configuration
                entity.Property(e => e.FirebaseUid).HasMaxLength(128).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
                
                // Optional fields with specific lengths
                entity.Property(e => e.DisplayName).HasMaxLength(256);
                entity.Property(e => e.PhotoUrl).HasMaxLength(500);
                
                // Timestamp configuration
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.LastLoginAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure relationship between User and Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Products)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Note: Seed data removed - products will be created per user after authentication
        }
    }
}