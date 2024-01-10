using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreQR.Models;

namespace StoreQR.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ClothingItem> ClothingItem { get; set; }
        public DbSet<StoringUnit> StoringUnit { get; set; }

        public DbSet<FamilyMember> FamilyMember { get; set; }

        //Ser till så att det returneras en tom lista om det inte finns några familjemedlemmar tillagda.

        //private ICollection<FamilyMember>? _familyMembers;

        //public ICollection<FamilyMember>? FamilyMembers
        //{
        //    get => _familyMembers ??= new List<FamilyMember>();
        //    set => _familyMembers = value;
        //}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Your configurations for ClothingItem entity
        //    modelBuilder.Entity<ClothingItem>()
        //        .Property(c => c.ClothingImage)
        //        .HasColumnType("varbinary(MAX)");


        //}
    }
}