using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using StoreQR.Interface;
using StoreQR.Models;
using System.Data;

namespace StoreQR.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DbSet<ClothingItem> ClothingItem { get; set; }
        public DbSet<StoringUnit> StoringUnit { get; set; }

        public DbSet<FamilyMember> FamilyMember { get; set; }
        //public DbSet<IdentityUser> IdentityUser { get; set; }

        //public IClothingFilterService ClothingFilterService { get; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor
            //IClothingFilterService clothingFilterService
            )
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            //ClothingFilterService = ClothingFilterService ?? throw new ArgumentNullException(nameof(clothingFilterService));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ClothingItem>()
                .HasKey(c => c.ClothingId);
            builder.Entity<ClothingItem>()
                .Property(c => c.ClothingImage)
                .HasColumnType("varbinary(MAX)");
        }
        public List<ClothingViewModel> GetFamilyMembersByUserId(string userId)
        {
            var familyMembers = new List<ClothingViewModel>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetFamilyMembersByUserId";
                command.CommandType = CommandType.StoredProcedure;

                //Lägg till parametrar för userid
                var userIdParameter = command.CreateParameter();
                userIdParameter.ParameterName = "@UserId";
                userIdParameter.DbType = DbType.String;
                userIdParameter.Value = userId;
                command.Parameters.Add( userIdParameter );

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
                                //QRCode = result.GetString(12),
                                StorageId = result.IsDBNull(13) ? (int?)null : result.GetInt32(13)
                            };

                            familyMembers.Add(familyMember);
                        }
                    }
                }
            }

            return familyMembers;
        }

        public List<ClothingViewModel> GetStorageNameByUserId(string userId)
        {
            var storageUnitNames = new List<ClothingViewModel>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetStorageNameByUserId";
                command.CommandType = CommandType.StoredProcedure;

                //Lägg till parametrar för userid
                var userIdParameter = command.CreateParameter();
                userIdParameter.ParameterName = "@UserId";
                userIdParameter.DbType = DbType.String;
                userIdParameter.Value = userId;
                command.Parameters.Add(userIdParameter);

                Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            var storageUnitName = new ClothingViewModel
                            {
                                StorageId = result.GetInt32(0),
                                StorageName = result.GetString(1),
                                UserId = result.GetString(2),
                            };

                            storageUnitNames.Add(storageUnitName);
                        }
                    }
                }
            }

            return storageUnitNames;
        }

        public List<ClothingViewModel> CombineFamilyNameAndStorageNameByUserId(string userId)
        {
            

            //Hämta familjemedlemmars namn och övrig info i clothingitem-tabellen
            var familyMembers = GetFamilyMembersByUserId(userId);
            //Hämta info om förvaringsutrymmen
            var storageUnits = GetStorageNameByUserId(userId);

            //Skapa en ordlista för att hålla koll på förvaringsutrymmens namn
            var storageNamesById = new Dictionary<int, string>();

            foreach (var storageUnit in storageUnits)
            {
                var storageId = storageUnit.StorageId.GetValueOrDefault();

                //Kolla om nyckel redan finns i lista
                if (!storageNamesById.ContainsKey(storageId))
                {
                    storageNamesById.Add(storageId, storageUnit.StorageName);
                }
            }


            var familyMembersAndStorage = familyMembers.Select(fm => new ClothingViewModel
            {
                ClothingId = fm.ClothingId,
                UserId = fm.UserId,
                ClothingName = fm.ClothingName,
                ClothingBrand = fm.ClothingBrand,
                ClothingSize = fm.ClothingSize,
                ClothingColor = fm.ClothingColor,
                Season = fm.Season,
                ClothingMaterial = fm.ClothingMaterial,
                TypeOfClothing = fm.TypeOfClothing,
                FamilyMemberName = fm.FamilyMemberName,
                FamilyUserId = fm.FamilyUserId,
                StorageId = fm.StorageId,
                //Använd ordlistan för att populera förvaringsutrymmen med namn
                StorageName = storageNamesById.TryGetValue(fm.StorageId.GetValueOrDefault(), out var storageName)
                    ? storageName
                    : "Ej angett"
            }).ToList();


            //  Lägg på förvaringsinfo på de kläder som har det inlagt.
            foreach (var storageUnit in storageUnits)
            {
                var matchingItem = familyMembersAndStorage.FirstOrDefault(c => c.StorageId == storageUnit.StorageId);

                if (matchingItem != null)
                {
                    matchingItem.StorageName = storageUnit.StorageName;
                } else
                {
                        // Sätt StorageName till "Ej angett" 
                        storageUnit.StorageName = "Ej angett";
                }

                

            }
            return familyMembersAndStorage;
        }

    }
}