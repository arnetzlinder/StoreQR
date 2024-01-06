using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreQR.Models;

namespace StoreQR.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ClothingItem> ClothingItem { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
