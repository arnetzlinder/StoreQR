using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreQR.Interface;
using StoreQR.Models;
using System.Data;

namespace StoreQR.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ClothingItem> ClothingItem { get; set; }
        public DbSet<StoringUnit> StoringUnit { get; set; }

        public DbSet<FamilyMember> FamilyMember { get; set; }
        
        //public IClothingFilterService ClothingFilterService { get; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options 
            //IClothingFilterService clothingFilterService
            )
            : base(options)
        {
            //ClothingFilterService = ClothingFilterService ?? throw new ArgumentNullException(nameof(clothingFilterService));
        }
        public List<ClothingViewModel> GetFamilyMembersForDropdown()
        {
            var familyMembers = new List<ClothingViewModel>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetFamilyMembersForDropdown";
                command.CommandType = CommandType.StoredProcedure;

                Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            var familyMember = new ClothingViewModel
                            {
                                ClothingId = result.GetInt32(0),
                                FamilyUserId = result.GetString(1),
                                FamilyMemberName = result.GetString(2),
                                UserId = result.GetString(3),
                                ClothingBrand = result.GetString(4),
                                ClothingColor = result.GetString(5),
                                //ClothingImage = result.GetString(6),
                                ClothingMaterial = result.GetString(7),
                                ClothingName = result.GetString(8),
                                ClothingSize = result.GetString(9),
                                Season = result.GetString(10),
                                TypeOfClothing = result.GetString(11),
                                //QRCode = result.GetString(12)
                            };

                            familyMembers.Add(familyMember);
                        }
                    }
                }
            }

            return familyMembers;
        }
    

    //Ser till så att det returneras en tom lista om det inte finns några familjemedlemmar tillagda.

    //private ICollection<FamilyMember>? _familyMembers;

    //public ICollection<FamilyMember>? FamilyMembers
    //{
    //    get => _familyMembers ??= new List<FamilyMember>();
    //    set => _familyMembers = value;
    //}

    

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