﻿using Microsoft.AspNetCore.Mvc;
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
            string FamilyMemberName
            );

        List<ClothingViewModel> GetAllClothingItems();
        List<ClothingViewModel> GetFamilyMembersByUserId(string userId);

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
        public List<ClothingViewModel> GetFamilyMembersByUserId(string userId)
        {
            var familyMembers = _context.GetFamilyMembersByUserId(userId);

            return familyMembers;
        }

        public List<ClothingViewModel> GetAllClothingItems()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != null)
            {
                var allClothingViewModels = _context.GetFamilyMembersByUserId(currentUserId)
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
                                   FamilyMemberName = c.FamilyMemberName,
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
            string FamilyMemberName
        )
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != null )
            {
                var clothingInfo = _context.GetFamilyMembersByUserId(currentUserId).ToList();
                var distinctBrands = clothingInfo.Select(c => c.ClothingBrand).Distinct().ToList();
                var distinctSizes = clothingInfo.Select(c => c.ClothingSize).Distinct().ToList();
                var distinctColors = clothingInfo.Select(c => c.ClothingColor).Distinct().ToList();
                var distinctSeasons = clothingInfo.Select(c => c.Season).Distinct().ToList();
                var distinctMaterials = clothingInfo.Select(c => c.ClothingMaterial).Distinct().ToList();
                var distinctTypeOfClothing = clothingInfo.Select(c => c.TypeOfClothing).Distinct().ToList();
                var distinctFamilyMemberName = clothingInfo.Select(c => c.FamilyMemberName).Distinct().ToList();
                var brandItems = distinctBrands.Select(brand => new SelectListItem { Text = brand, Value = brand }).ToList();
                var sizeItems = distinctSizes.Select(size => new SelectListItem { Text = size, Value = size }).ToList();
                var colorItems = distinctColors.Select(color => new SelectListItem { Text = color, Value = color }).ToList();
                var seasonItems = distinctSeasons.Select(season => new SelectListItem { Text = season, Value = season }).ToList();
                var materialItems = distinctMaterials.Select(material => new SelectListItem { Text = material, Value = material }).ToList();
                var typeItems = distinctTypeOfClothing.Select(type => new SelectListItem { Text = type, Value = type }).ToList();
                var nameItems = distinctFamilyMemberName.Select(name => new SelectListItem { Text = name, Value = name }).ToList();
                // Kollar om det finns något innehåll och isf filtrerar på vald egenskap
                var query = _context.GetFamilyMembersByUserId(currentUserId).AsQueryable();

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
                if (!string.IsNullOrEmpty(FamilyMemberName))
                {
                    query = query.Where(c => string.Equals(c.FamilyMemberName, FamilyMemberName, StringComparison.OrdinalIgnoreCase));
                }


                // Genomför förfrågan och returnera resultaten
                var filteredClothingViewModels = _context.GetFamilyMembersByUserId(currentUserId)
                .Where(c =>
               (string.IsNullOrEmpty(ClothingBrand) || c.ClothingBrand == ClothingBrand) &&
               (string.IsNullOrEmpty(ClothingSize) || c.ClothingSize == ClothingSize) &&
               (string.IsNullOrEmpty(ClothingColor) || c.ClothingColor == ClothingColor) &&
               (string.IsNullOrEmpty(Season) || c.Season == Season) &&
               (string.IsNullOrEmpty(ClothingMaterial) || c.ClothingMaterial == ClothingMaterial) &&
               (string.IsNullOrEmpty(TypeOfClothing) || c.TypeOfClothing == TypeOfClothing) &&
               (string.IsNullOrEmpty(FamilyMemberName) || c.FamilyMemberName == FamilyMemberName)
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
                    FamilyMemberName = c.FamilyMemberName
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
