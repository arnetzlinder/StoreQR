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
using Microsoft.Exchange.WebServices.Data;


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
                var distinctBrands = clothingInfo.Select(c => c.ClothingBrand).Distinct().ToList();
                var distinctSizes = clothingInfo.Select(c => c.ClothingSize).Distinct().ToList();
                var distinctColors = clothingInfo.Select(c => c.ClothingColor).Distinct().ToList();
                var distinctSeasons = clothingInfo.Select(c => c.Season).Distinct().ToList();
                var distinctMaterials = clothingInfo.Select(c => c.ClothingMaterial).Distinct().ToList();
                var distinctTypeOfClothing = clothingInfo.Select(c => c.TypeOfClothing).Distinct().ToList();
                var distinctFamilyMemberName = clothingInfo.Select(c => c.FamilyMemberName).Distinct().ToList();
                var distinctStorageNames = clothingInfo.Select(c => c.StorageName).Distinct().ToList();
                var brandItems = distinctBrands.Select(brand => new SelectListItem { Text = brand, Value = brand }).ToList();
                var sizeItems = distinctSizes.Select(size => new SelectListItem { Text = size, Value = size }).ToList();
                var colorItems = distinctColors.Select(color => new SelectListItem { Text = color, Value = color }).ToList();
                var seasonItems = distinctSeasons.Select(season => new SelectListItem { Text = season, Value = season }).ToList();
                var materialItems = distinctMaterials.Select(material => new SelectListItem { Text = material, Value = material }).ToList();
                var typeItems = distinctTypeOfClothing.Select(type => new SelectListItem { Text = type, Value = type }).ToList();
                var nameItems = distinctFamilyMemberName.Select(name => new SelectListItem { Text = name, Value = name }).ToList();
                var storingItems = distinctStorageNames.Select(storage => new SelectListItem { Text = storage, Value = storage }).ToList();
                // Kollar om det finns något innehåll och isf filtrerar på vald egenskap
                var query = _context.CombineFamilyNameAndStorageNameByUserId(currentUserId).AsQueryable();

                if (!string.IsNullOrEmpty(ClothingBrand))
                {
                    query = query.Where(c => c.ClothingBrand == ClothingBrand);
                }

                if (!string.IsNullOrEmpty(ClothingSize))
                {
                    query = query.Where(c => c.ClothingSize == ClothingSize);
                }

                if (!string.IsNullOrEmpty(ClothingColor))
                {
                    query = query.Where(c => c.ClothingColor == ClothingColor);
                }

                if (!string.IsNullOrEmpty(Season))
                {
                    query = query.Where(c => c.Season == Season);
                }

                if (!string.IsNullOrEmpty(ClothingMaterial))
                {
                    query = query.Where(c => c.ClothingMaterial == ClothingMaterial);
                }

                if (!string.IsNullOrEmpty(TypeOfClothing))
                {
                    query = query.Where(c => c.TypeOfClothing == TypeOfClothing);
                }
                if (!string.IsNullOrEmpty(FamilyMemberName))
                {
                    query = query.Where(c => c.FamilyMemberName == FamilyMemberName);
                }
                if (!string.IsNullOrEmpty(StorageName))
                {
                    query = query.Where(c => c.StorageName == StorageName);
                }


                // Genomför förfrågan och returnera resultaten
                var filteredClothingViewModels = _context.CombineFamilyNameAndStorageNameByUserId(currentUserId)
                .Where(c =>
               (string.IsNullOrEmpty(ClothingBrand) || c.ClothingBrand == ClothingBrand) &&
               (string.IsNullOrEmpty(ClothingSize) || c.ClothingSize == ClothingSize) &&
               (string.IsNullOrEmpty(ClothingColor) || c.ClothingColor == ClothingColor) &&
               (string.IsNullOrEmpty(Season) || c.Season == Season) &&
               (string.IsNullOrEmpty(ClothingMaterial) || c.ClothingMaterial == ClothingMaterial) &&
               (string.IsNullOrEmpty(TypeOfClothing) || c.TypeOfClothing == TypeOfClothing) &&
               (string.IsNullOrEmpty(FamilyMemberName) || c.FamilyMemberName == FamilyMemberName) &&
               (string.IsNullOrEmpty(StorageName) || c.StorageName == StorageName)
                )
                .Select(c => new ClothingViewModel
                {
                    //Eftersom vi har en join så mappar vi ClothingItem mot ClothingViewModel
                    ClothingId = c.ClothingId,
                    ClothingName = c.ClothingName,
                    ClothingBrand = c.ClothingBrand,
                    ClothingSize = c.ClothingSize,
                    ClothingColor = c.ClothingColor,
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
