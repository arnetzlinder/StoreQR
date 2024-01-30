using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using StoreQR.Interface;
using StoreQR.Models;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
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
            builder.Entity<FamilyMember>()
                .HasKey(fm => fm.Id);
            builder.Entity<ClothingItem>()
                .Property(c => c.ClothingImage)
                .HasColumnType("varchar(MAX)");
            builder.Entity<StoringUnit>()
                .Property(s => s.StorageImage)
                .HasColumnType("varchar(MAX)");
        }

        //Relaterat till Kläder
        public List<ClothingViewModel> GetFamilyMembersByUserId(string userId)
        {
            var familyMembers = new List<ClothingViewModel>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetClothingItemWithFamilyMemberNameByUserId";
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
                                ClothingBrand = result.IsDBNull(4) ? null! : result.GetString(4),
                                ClothingColor = result.IsDBNull(5) ? null! : result.GetString(5),
                                ClothingImage = result.IsDBNull(6) ? null! : result.GetString(6),
                                ClothingMaterial = result.IsDBNull(7) ? null! : result.GetString(7),
                                ClothingName = result.GetString(8),
                                ClothingSize = result.IsDBNull(9) ? null! : result.GetString(9),
                                Season = result.IsDBNull(10) ? null! : result.GetString(10),
                                TypeOfClothing = result.IsDBNull(11) ? null! : result.GetString(11),
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
                    //Om värdet redan finns i listan. lägg inte till igen (eftersom det kan ligga många saker i ett förvaringsutrymme)
                    if (!storageNamesById.ContainsValue(storageUnit.StorageName))
                    {
                        storageNamesById.Add(storageId, storageUnit.StorageName);
                    }
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
                }
                else
                {
                    // Sätt StorageName till "Ej angett" 
                    storageUnit.StorageName = "Ej angett";
                }



            }
            return familyMembersAndStorage;
        }
        public async Task<List<ClothingItem>> GetClothingItemAsync(int ClothingId, string UserId)
        {
            var familyMemberNameTask = GetClothingItemWithFamilyMemberNameAsync(ClothingId);
            var storageNameTask = GetClothingItemWithStorageNameAsync(ClothingId);

            var familyMemberName = (await GetClothingItemWithFamilyMemberNameAsync(ClothingId)).FirstOrDefault();
            var storageName = (await GetClothingItemWithStorageNameAsync(ClothingId)).FirstOrDefault();


            var clothingItem = new List<ClothingItem>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetClothingItemById";
                command.CommandType = CommandType.StoredProcedure;

                var clothingItemParameter = command.CreateParameter();
                clothingItemParameter.ParameterName = "@ClothingId";
                clothingItemParameter.DbType = DbType.Int32;
                clothingItemParameter.Value = ClothingId;
                command.Parameters.Add(clothingItemParameter);

                clothingItemParameter = command.CreateParameter();
                clothingItemParameter.ParameterName = "@UserId";
                clothingItemParameter.DbType = DbType.String;
                clothingItemParameter.Value = UserId;
                command.Parameters.Add(clothingItemParameter);

                await Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            var clothingItemValues = new ClothingItem()
                            {
                                ClothingId = result.GetInt32(0),
                                ClothingImage = result.IsDBNull(2) ? null : result.GetString(2),
                                ClothingName = result.GetString(3),
                                ClothingUserId = result.IsDBNull(4) ? (int?)null : (result.GetInt32(4) == -1 ? (int?)null : result.GetInt32(4)),
                                // QRCode = result.IsDBNull(5) ? null : result.GetString(5),
                                ClothingBrand = result.IsDBNull(6) ? null : result.GetString(6),
                                ClothingSize = result.IsDBNull(7) ? null : result.GetString(7),
                                ClothingColor = result.IsDBNull(8) ? null : result.GetString(8),
                                Season = result.IsDBNull(9) ? null : result.GetString(9),
                                ClothingMaterial = result.IsDBNull(10) ? null : result.GetString(10),
                                TypeOfClothing = result.IsDBNull(11) ? null : result.GetString(11),
                                StorageId = result.IsDBNull(12) ? (int?)null : result.GetInt32(12)
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
                                FamilyMemberName = result.IsDBNull(1) ? "Ingen vald" : result.GetString(1),
                                FamilyMemberId = result.IsDBNull(2) ? -1 : result.GetInt32(2)
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
                                StorageName = result.GetString(1),
                                StorageId = result.GetInt32(2)
                            };

                            clothingItemWithStorageName.Add(clothingFamilyName);
                        }
                    }
                }
                Database.CloseConnection();
            }

            return clothingItemWithStorageName;
        }


        //Relaterat till familjemedlemmar
        public List<FamilyMember> GetFamilyMembersFilteredByUserId (string userId)
        {
            var familyMembers = new List<FamilyMember>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetFamilyMembersFilteredByUserId";
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
                    if(result.HasRows)
                    {
                        while (result.Read())
                        {
                            var familyMember = new FamilyMember
                            {
                                Id = result.GetInt32(0),
                                UserId = result.GetString(1),
                                Name = result.GetString(2)
                            };
                            familyMembers.Add(familyMember);
                        }
                    }
                }
                Database.CloseConnection();
            }
            return familyMembers;
        }

        public async Task<List<FamilyMember>> GetFamilyMemberAsync(int Id, string UserId)
        {

            var familyMember = new List<FamilyMember>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetFamilyMemberById";
                command.CommandType = CommandType.StoredProcedure;

                var storingUnitParameter = command.CreateParameter();
                storingUnitParameter.ParameterName = "@Id";
                storingUnitParameter.DbType = DbType.Int32;
                storingUnitParameter.Value = Id;
                command.Parameters.Add(storingUnitParameter);

                storingUnitParameter = command.CreateParameter();
                storingUnitParameter.ParameterName = "@UserId";
                storingUnitParameter.DbType = DbType.String;
                storingUnitParameter.Value = UserId;
                command.Parameters.Add(storingUnitParameter);

                await Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            var familyMemberValues = new FamilyMember()
                            {
                                Id = result.GetInt32(0),
                                Name = result.GetString(2)
                            };


                            familyMember.Add(familyMemberValues);
                        }
                    }
                }
                await Database.CloseConnectionAsync();
            }


            return familyMember;
        }

        public async Task<IEnumerable<FamilyMember>> DeleteFamilyMemberByIdAsync(int id, string userId)
        {
            var familyMembers = new List<FamilyMember>();
            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "DeleteFamilyMemberById";
                command.CommandType = CommandType.StoredProcedure;

                var familyMemberParameter = command.CreateParameter();
                familyMemberParameter.ParameterName = "@Id";
                familyMemberParameter.DbType = DbType.Int32;
                familyMemberParameter.Value = id;
                command.Parameters.Add(familyMemberParameter);

                familyMemberParameter = command.CreateParameter();
                familyMemberParameter.ParameterName = "@UserId";
                familyMemberParameter.DbType = DbType.String;
                familyMemberParameter.Value = userId;
                command.Parameters.Add(familyMemberParameter);

                await Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            var familyMember = new FamilyMember()
                            {
                                Id = result.GetInt32(0),
                                Name = result.GetString(2)
                            };

                            familyMembers.Add(familyMember);
                        }
                    }
                }

                await Database.CloseConnectionAsync();
            }


            return familyMembers;
        }

        //Relaterat till förvaringsutrymmen
        public List<StoringUnit> GetStorageUnitsFilteredByUserId(string userId)
        {
            var storingUnits = new List<StoringUnit>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetStorageUnitsFilteredByUserId";
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
                            var storingUnit = new StoringUnit
                            {
                                StorageId = result.GetInt32(0),
                                StorageName = result.GetString(1),
                                StorageDescription = result.GetString(2),
                                UserId = result.GetString(3),
                                StorageImage = result.IsDBNull(4) ? null! : result.GetString(4),
                            };
                            storingUnits.Add(storingUnit);
                        }
                    }
                }
                Database.CloseConnection();
            }
            return storingUnits;
        }

       
        public async Task<List<StoringUnit>> GetStoringUnitAsync(int StorageId, string UserId)
        {

            var storingUnit = new List<StoringUnit>();

            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetStoringUnitById";
                command.CommandType = CommandType.StoredProcedure;

                var storingUnitParameter = command.CreateParameter();
                storingUnitParameter.ParameterName = "@StorageId";
                storingUnitParameter.DbType = DbType.Int32;
                storingUnitParameter.Value = StorageId;
                command.Parameters.Add(storingUnitParameter);

                storingUnitParameter = command.CreateParameter();
                storingUnitParameter.ParameterName = "@UserId";
                storingUnitParameter.DbType = DbType.String;
                storingUnitParameter.Value = UserId;
                command.Parameters.Add(storingUnitParameter);

                await Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            var storingUnitValues = new StoringUnit()
                            {
                                StorageId = result.GetInt32(0),
                                //StorageImage = result.GetString(2),
                                StorageName = result.GetString(3),
                                StorageDescription = result.GetString(4)
                                //QRCode = result.GetString (5)
                            };


                            storingUnit.Add(storingUnitValues);
                        }
                    }
                }
                await Database.CloseConnectionAsync();
            }


            return storingUnit;
        }

        public async Task<bool> DeleteStorageUnitByIdAsync(int storageId, string userId)
        {
            var storageUnits = new List<StoringUnit>();
            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "DeleteStorageUnitById";
                command.CommandType = CommandType.StoredProcedure;

                var storingUnitParameter = command.CreateParameter();
                storingUnitParameter.ParameterName = "@StorageId";
                storingUnitParameter.DbType = DbType.Int32;
                storingUnitParameter.Value = storageId;
                command.Parameters.Add(storingUnitParameter);

                storingUnitParameter = command.CreateParameter();
                storingUnitParameter.ParameterName = "@UserId";
                storingUnitParameter.DbType = DbType.String;
                storingUnitParameter.Value = userId;
                command.Parameters.Add(storingUnitParameter);

                await Database.OpenConnectionAsync();

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return true; // Deletion successful
                }
                catch (Exception)
                {
                    return false; // Deletion failed
                }
                finally
                {
                    await Database.CloseConnectionAsync();
                }
            }
        }
    }
      
    }


