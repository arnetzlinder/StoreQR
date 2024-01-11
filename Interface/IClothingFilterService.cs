using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using StoreQR.Data;
using StoreQR.Models;

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
            string TypeOfClothing
            );

        List<ClothingViewModel> GetAllClothingItems();
      
    }

    public class ClothingFilterService : IClothingFilterService
    {
        private readonly ApplicationDbContext _context;

        public ClothingFilterService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<ClothingViewModel> GetAllClothingItems()
        {
            var allClothingViewModels = _context.ClothingItem
                .Select(c => new ClothingViewModel
                {
                    ClothingId = c.ClothingId,
                    ClothingName = c.ClothingName,
                    ClothingBrand = c.ClothingBrand,
                    ClothingSize = c.ClothingSize,
                    ClothingColor = c.ClothingColor,
                    Season = c.Season,
                    ClothingMaterial = c.ClothingMaterial,
                    TypeOfClothing = c.TypeOfClothing,
                })
                .ToList();

            return allClothingViewModels;
        }
        public List<ClothingViewModel> GetFilteredClothingItems(
            string ClothingBrand,
            string ClothingSize,
            string ClothingColor,
            string Season,
            string ClothingMaterial,
            string TypeOfClothing
        )
        {
            var distinctBrands = _context.ClothingItem.Select(c => c.ClothingBrand).Distinct().ToList();
            var distinctSizes = _context.ClothingItem.Select(c => c.ClothingSize).Distinct().ToList();
            var distinctColors = _context.ClothingItem.Select(c => c.ClothingColor).Distinct().ToList();
            var distinctSeasons = _context.ClothingItem.Select(c => c.Season).Distinct().ToList();
            var distinctMaterials = _context.ClothingItem.Select(c => c.ClothingMaterial).Distinct().ToList();
            var distinctTypeOfClothing = _context.ClothingItem.Select(c => c.TypeOfClothing).Distinct().ToList();
            var brandItems = distinctBrands.Select(brand => new SelectListItem { Text = brand, Value = brand }).ToList();
            var sizeItems = distinctSizes.Select(size => new SelectListItem { Text = size, Value = size }).ToList();
            var colorItems = distinctColors.Select(color => new SelectListItem { Text = color, Value = color }).ToList();
            var seasonItems = distinctSeasons.Select(season => new SelectListItem { Text = season, Value = season }).ToList();
            var materialItems = distinctMaterials.Select(material => new SelectListItem { Text = material, Value = material }).ToList();
            var typeItems = distinctTypeOfClothing.Select(type => new SelectListItem { Text = type, Value = type }).ToList();
            // Kollar om det finns något innehåll och isf filtrerar på vald egenskap
            var query = _context.ClothingItem.AsQueryable();

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
                query = query.Where(c => string.Equals(c.ClothingColor, ClothingColor, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(Season))
            {
                query = query.Where(c => string.Equals(c.Season, Season, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(ClothingMaterial))
            {
                query = query.Where(c => string.Equals(c.ClothingMaterial, ClothingMaterial, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(TypeOfClothing))
            {
                query = query.Where(c => string.Equals(c.TypeOfClothing, TypeOfClothing, StringComparison.OrdinalIgnoreCase));
            }


            // Genomför förfrågan och returnera resultaten
            var filteredClothingViewModels = query
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
               })
               .ToList();

            return filteredClothingViewModels;
        }
    }
}
