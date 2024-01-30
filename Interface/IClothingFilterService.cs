using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;


namespace StoreQR.Interface
{
    public interface IClothingFilterService
    {
        //Skapar en lista för att filtrera kläder på olika egenskaper
        List<ClothingViewModel> GetFilteredClothingItems(
            string ClothingBrand,
            string ClothingSize,
            string ClothingColor,
            string Season,
            string ClothingMaterial,
            string TypeOfClothing,
            string FamilyMemberName,
            string StorageName
            );

        List<ClothingViewModel> GetAllClothingItems();
        List<ClothingViewModel> CombineFamilyNameAndStorageNameByUserId(string userId);

    }

    public class ClothingFilterService : IClothingFilterService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClothingFilterService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public List<ClothingViewModel> CombineFamilyNameAndStorageNameByUserId(string userId)
        {
            var familyMembers = _context.CombineFamilyNameAndStorageNameByUserId(userId);

            return familyMembers;
        }

        public List<ClothingViewModel> GetAllClothingItems()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != null)
            {
                var allClothingViewModels = _context.CombineFamilyNameAndStorageNameByUserId(currentUserId)
                               .Select(c => new ClothingViewModel
                               {
                                   ClothingId = c.ClothingId,
                                   ClothingName = c.ClothingName,
                                   ClothingBrand = c.ClothingBrand,
                                   ClothingSize = c.ClothingSize,
                                   ClothingColor = c.ClothingColor,
                                   ClothingImage = c.ClothingImage,
                                   Season = c.Season,
                                   ClothingMaterial = c.ClothingMaterial,
                                   TypeOfClothing = c.TypeOfClothing,
                                   FamilyMemberName = c.FamilyMemberName,
                                   StorageId = c.StorageId,
                                   StorageName = c.StorageName,
                               })
                               .ToList();

                return allClothingViewModels;
            } else
            {
                Console.WriteLine("Something went wrong");
                throw new Exception("Something went wrong");
            }
           
        }
        public List<ClothingViewModel> GetFilteredClothingItems(
            string ClothingBrand,
            string ClothingSize,
            string ClothingColor,
            string Season,
            string ClothingMaterial,
            string TypeOfClothing,
            string FamilyMemberName,
            string StorageName
        )
        {
            //Hämtar och filtrerar ut olika egenskaper utifrån vad användaren har lagt till i databasen
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != null )
            {
                var clothingInfo = _context.CombineFamilyNameAndStorageNameByUserId(currentUserId).ToList();
                var filterOptions = new List<Func<ClothingViewModel, bool>>();

                if (!string.IsNullOrEmpty(ClothingBrand))
                    filterOptions.Add(c => c.ClothingBrand == ClothingBrand);

                if (!string.IsNullOrEmpty(ClothingSize))
                    filterOptions.Add(c => c.ClothingSize == ClothingSize);

                if (!string.IsNullOrEmpty(ClothingColor))
                    filterOptions.Add(c => c.ClothingColor == ClothingColor);

                if (!string.IsNullOrEmpty(Season))
                    filterOptions.Add(c => c.Season == Season);

                if (!string.IsNullOrEmpty(ClothingMaterial))
                    filterOptions.Add(c => c.ClothingMaterial == ClothingMaterial);

                if (!string.IsNullOrEmpty(TypeOfClothing))
                    filterOptions.Add(c => c.TypeOfClothing == TypeOfClothing);

                if (!string.IsNullOrEmpty(FamilyMemberName))
                    filterOptions.Add(c => c.FamilyMemberName == FamilyMemberName);

                if (!string.IsNullOrEmpty(StorageName))
                    filterOptions.Add(c => c.StorageName == StorageName);



                // Genomför förfrågan och returnera resultaten
                var filteredClothingViewModels = clothingInfo
                    .Where(c => filterOptions.All(option => option(c)))
                    .OrderBy(c => c.ClothingSize ?? "")
                    .ThenBy(c => c.ClothingBrand ?? "")
                    .ThenBy(c => c.ClothingColor ?? "")
                    .ThenBy(c => c.Season ?? "")
                    .ThenBy(c => c.ClothingMaterial ?? "")
                    .ThenBy(c => c.TypeOfClothing ?? "")
                    .ThenBy(c=> c.FamilyMemberName ?? "")
                    .ThenBy (c => c.StorageName ?? "")
                    .Select(c => new ClothingViewModel
                {
                    
                    ClothingId = c.ClothingId,
                    ClothingName = c.ClothingName,
                    ClothingBrand = c.ClothingBrand,
                    ClothingSize = c.ClothingSize,
                    ClothingColor = c.ClothingColor,
                    ClothingImage = c.ClothingImage,
                    Season = c.Season,
                    ClothingMaterial = c.ClothingMaterial,
                    TypeOfClothing = c.TypeOfClothing,
                    FamilyMemberName = c.FamilyMemberName,
                    StorageName = c.StorageName
                })
                .ToList();
                return filteredClothingViewModels;
            } else
            {
                Console.WriteLine("Something went wrong");
                throw new Exception("Something went wrong");
            }
           
        }
    }
}
