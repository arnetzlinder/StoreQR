using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client.Extensions.Msal;
using StoreQR.Interface;
using StoreQR.Models;
using System.Data;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StoreQR.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DbSet<ClothingItem> ClothingItem { get; set; }
        public DbSet<StoringUnit> StoringUnit { get; set; }

        public DbSet<FamilyMember> FamilyMember { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor
            //IClothingFilterService clothingFilterService
            )
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
                                ClothingImage = GetBytes(result, 6),
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
                Database.CloseConnection();
            }

            return familyMembers;
        }

        //För att få ut bildfil från byte []
        private static byte[]? GetBytes(DbDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null; // Returnera null om null i databas så att exempelbild istället visas i frontenden
            }

            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            long bytesRead;
            long fieldOffset = 0;

            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(ordinal, fieldOffset, buffer, 0, bufferSize)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bufferSize;
                }

                return stream.ToArray();
            }
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
                Database.CloseConnection();
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
                ClothingImage = fm.ClothingImage,
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
        public async Task<List<ClothingViewModel>> GetClothingItemAsync(int ClothingId)
        {
            var familyMemberNameTask = GetClothingItemWithFamilyMemberNameAsync(ClothingId);
            var storageNameTask = GetClothingItemWithStorageNameAsync(ClothingId);

            var familyMemberName = (await GetClothingItemWithFamilyMemberNameAsync(ClothingId)).FirstOrDefault();
            var storageName = (await GetClothingItemWithStorageNameAsync(ClothingId)).FirstOrDefault();


            var clothingItem = new List<ClothingViewModel>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetClothingItemById";
                command.CommandType = CommandType.StoredProcedure;

                var clothingItemParameter = command.CreateParameter();
                clothingItemParameter.ParameterName = "@ClothingId";
                clothingItemParameter.DbType = DbType.Int32;
                clothingItemParameter.Value = ClothingId;
                command.Parameters.Add(clothingItemParameter);

                await Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            var clothingItemValues = new ClothingViewModel()
                            {
                                ClothingId = result.GetInt32(0),
                                ClothingName = result.GetString(2),
                                ClothingUserId = result.GetInt32(3),
                                //QRCode = result.GetString (4),
                                ClothingBrand = result.GetString(5),
                                ClothingSize = result.GetString(6),
                                ClothingColor = result.GetString(7),
                                Season = result.GetString(8),
                                ClothingMaterial = result.GetString(9),
                                TypeOfClothing = result.GetString(10),
                                StorageId = result.GetInt32(11),
                                FamilyMemberName = familyMemberName?.FamilyMemberName,
                                StorageName = storageName?.StorageName
                            };
                            

                            clothingItem.Add(clothingItemValues);
                        }
                    }
                }
                await Database.CloseConnectionAsync();
            }
           

            return clothingItem;
        }

        public async Task<List<ClothingViewModel>> GetClothingItemWithFamilyMemberNameAsync(int ClothingId)
        {
            var clothingItemWithFamilyMemberName = new List<ClothingViewModel>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetClothingItemWithFamilyMemberName";
                command.CommandType = CommandType.StoredProcedure;

                var clothingItemParameter = command.CreateParameter();
                clothingItemParameter.ParameterName = "@ClothingId";
                clothingItemParameter.DbType = DbType.Int32;
                clothingItemParameter.Value = ClothingId;
                command.Parameters.Add(clothingItemParameter);

                await Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            var clothingFamilyName = new ClothingViewModel()
                            {
                                ClothingId = result.GetInt32(0),
                                FamilyMemberName = result.GetString(13),
                                FamilyMemberId = result.GetInt32(14)
                            };

                            clothingItemWithFamilyMemberName.Add(clothingFamilyName);
                        }
                    }
                }
                Database.CloseConnection();
            }
            
            return clothingItemWithFamilyMemberName;
        }

        public async Task<List<ClothingViewModel>> GetClothingItemWithStorageNameAsync(int ClothingId)
        {
            var clothingItemWithStorageName = new List<ClothingViewModel>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetClothingItemWithStorageName";
                command.CommandType = CommandType.StoredProcedure;

                var clothingItemParameter = command.CreateParameter();
                clothingItemParameter.ParameterName = "@ClothingId";
                clothingItemParameter.DbType = DbType.Int32;
                clothingItemParameter.Value = ClothingId;
                command.Parameters.Add(clothingItemParameter);

                await Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            var clothingFamilyName = new ClothingViewModel()
                            {
                                ClothingId = result.GetInt32(0),
                                StorageName = result.GetString(13),
                                StorageId = result.GetInt32(14)
                            };

                            clothingItemWithStorageName.Add(clothingFamilyName);
                        }
                    }
                }
                Database.CloseConnection();
            }
            
            return clothingItemWithStorageName;
        }

    }
      
    }


