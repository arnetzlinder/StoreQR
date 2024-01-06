using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreQR.Models;

namespace StoreQR.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ClothingItem> ClothingItem { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClothingItem>()
                .Property(c => c.ClothingImage)
                .HasColumnType("varbinary(MAX)");
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
