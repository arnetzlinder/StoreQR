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
using StoreQR.Interface;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using X.PagedList;

namespace StoreQR.Controllers
{
    [Authorize]
    public class ClothingController : Controller
    {
        private readonly ILogger<ClothingController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IClothingFilterService _filterService;
        private readonly UserManager<IdentityUser> _userManager;


        public ClothingController(ILogger<ClothingController> logger, 
            ApplicationDbContext context, 
            IClothingFilterService filterService,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        //[Authorize]
        public IActionResult Index(ClothingViewModel viewModel, bool? ResetFilters, int? page)
        {
            //Hämta användarens id med hjälp av user manager
            string? currentUserId = _userManager.GetUserId(HttpContext.User);
            if (currentUserId != null)
            {
                //Hämtar alla värden först
                var familyMembersAndStorage = _context.CombineFamilyNameAndStorageNameByUserId(currentUserId);
                
                //Sidor i pagineringen
                int pageSize = 9;

                //Plockar ut distinkta värden för familjemedlemmar
                var distinctFamilyMembers = familyMembersAndStorage
                    .Select(c => new
                    {
                        ClothingBrand = c.ClothingBrand,
                        ClothingSize = c.ClothingSize,
                        ClothingColor = c.ClothingColor,
                        Season = c.Season,
                        ClothingMaterial = c.ClothingMaterial,
                        TypeOfClothing = c.TypeOfClothing,
                        FamilyMemberName = c.FamilyMemberName,
                        ClothingImage = c.ClothingImage,
                    })
                    .Distinct()
                    .ToList();
                
                //Plockar ut distinkta värden för förvaringsutrymmen
                var storageUnits = familyMembersAndStorage.Select(c => new
                {
                    StorageId = c.StorageId,
                    StorageName = c.StorageName,
                    
                })
                    .Distinct()
                    .ToList();
                

                //var clothingInfo = _context.GetFamilyMembersByUserId(currentUserId).ToList();
                //var storageUnitNames = _context.GetStorageNameByUserId(currentUserId).ToList();
                ViewBag.DistinctBrands = distinctFamilyMembers.Select(c => c.ClothingBrand).Distinct().OrderBy(x => x).ToList();
                ViewBag.DistinctSizes = distinctFamilyMembers.Select(c => c.ClothingSize).Distinct().OrderBy(x => x).ToList();
                ViewBag.DistinctColors = distinctFamilyMembers.Select(c => c.ClothingColor).Distinct().OrderBy(x => x).ToList();
                ViewBag.DistinctSeasons = distinctFamilyMembers.Select(c => c.Season).Distinct().OrderBy(x => x).ToList();
                ViewBag.DistinctMaterials = distinctFamilyMembers.Select(c => c.ClothingMaterial).Distinct().OrderBy(x => x).ToList();
                ViewBag.DistinctTypesOfClothing = distinctFamilyMembers.Select(c => c.TypeOfClothing).Distinct().OrderBy(x => x).ToList();
                ViewBag.DistinctFamilyMemberName = distinctFamilyMembers.Select(c => c.FamilyMemberName).Distinct().OrderBy(x => x).ToList();
                ViewBag.StorageUnitName = storageUnits.Select(c => c.StorageName).Distinct().OrderBy(x => x).ToList();

               

                if (ResetFilters.HasValue && ResetFilters.Value)
                {
                    // Återställ filter om knappen återställ klickats på
                    viewModel.FamilyMemberName = "";
                    viewModel.ClothingBrand = "";
                    viewModel.ClothingSize = "";
                    viewModel.ClothingColor = "";
                    viewModel.Season = "";
                    viewModel.ClothingMaterial = "";
                    viewModel.TypeOfClothing = "";
                    viewModel.StorageName = "";
                }


                //Om alla fält är tomma eller alla är vald
                if (viewModel.ClothingBrand == "" &&
                    viewModel.ClothingSize == "" &&
                    viewModel.ClothingColor == "" &&
                    viewModel.Season == "" &&
                    viewModel.ClothingMaterial == "" &&
                    viewModel.TypeOfClothing == "" &&
                    viewModel.FamilyMemberName == "" &&
                    viewModel.StorageName == "")
                {
                    //Hämtar alla träffar om inget val gjorts eller om användaren väljer alla märken
                    var allClothingItems = _filterService.GetAllClothingItems();
                    var pagedClothingItems = allClothingItems.ToPagedList(page ?? 1, pageSize);

                    var pagedClothingViewModels = new StaticPagedList<ClothingViewModel>(
                        pagedClothingItems.Select(item => new ClothingViewModel 
                        {
                            ClothingId = item.ClothingId,
                            ClothingName = item.ClothingName,
                            ClothingImage = item.ClothingImage,
                            ClothingBrand = item.ClothingBrand,
                            ClothingSize = item.ClothingSize,
                            ClothingColor = item.ClothingColor,
                            Season = item.Season,
                            ClothingMaterial = item.ClothingMaterial,
                            TypeOfClothing = item.TypeOfClothing,
                            FamilyMemberName = item.FamilyMemberName,
                            StorageName = item.StorageName
                        }),
                        pagedClothingItems.GetMetaData()
                    );
                    return View(pagedClothingViewModels);
                }
                else
                {
                    var filteredClothingItems = _filterService.GetFilteredClothingItems(
                                  viewModel.ClothingBrand,
                                  viewModel.ClothingSize,
                                  viewModel.ClothingColor,
                                  viewModel.Season,
                                  viewModel.ClothingMaterial,
                                  viewModel.TypeOfClothing,
                                  viewModel.FamilyMemberName,
                                  viewModel.StorageName
                              );



                    var pagedFilteredClothingItems = filteredClothingItems.ToPagedList(page ?? 1, pageSize);

                    var pagedFilteredClothingViewModels = new StaticPagedList<ClothingViewModel>(
                        pagedFilteredClothingItems.Select(item => new ClothingViewModel { 
                        ClothingId = item.ClothingId,
                        ClothingName = item.ClothingName,
                        ClothingImage = item.ClothingImage,
                        ClothingBrand = item.ClothingBrand,
                        ClothingSize = item.ClothingSize,
                        ClothingColor = item.ClothingColor,
                        Season = item.Season,
                        ClothingMaterial = item.ClothingMaterial,
                        TypeOfClothing = item.TypeOfClothing,
                        FamilyMemberName = item.FamilyMemberName,
                        StorageName = item.StorageName
                        }),
                        pagedFilteredClothingItems.GetMetaData()
                    );

                    return View(pagedFilteredClothingViewModels);
                }
            } else
            {
                Console.WriteLine("Something went wrong");
                throw new Exception("Something went wrong");
            }
           
          
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var familyMemberNames = _context.FamilyMember
                    .Where(fm => fm.UserId == userId)
                    .Select(fm => new SelectListItem
                    {
                        Value = fm.Id.ToString(),
                        Text = fm.Name
                    })
                    .Distinct()
                    .ToList();

                if (familyMemberNames.Any())
                {
                    ViewBag.FamilyMemberNames = new SelectList(familyMemberNames, "Value", "Text");
                }
                else
                {
                    ViewBag.FamilyMemberNames = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Inga medlemmar i hushållet tillagda" }
            };
                }
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var storageData = _context.GetStorageNameByUserId(userId)
                    .Where(fm => fm.UserId == userId)
                    .GroupBy(fm => fm.StorageName)
                    .Select(group => group.First())
                    .Select(fm => new SelectListItem
                    {
                        Value = fm.StorageId.ToString() ?? "",
                        Text = fm.StorageName
                    })
                    .ToList();

                var qrCodes = _context.GetStorageNameByUserId(userId)
                    .Where(fm => fm.UserId == userId)
                    .GroupBy(fm => fm.StorageName)
                    .Select(group => group.First())
                    .Select(fm => new SelectListItem
                    {
                        Value = fm.QRCode ?? "",
                        Text = fm.StorageId.ToString()
                    })
                    .ToList();


                if (storageData.Any())
                {
                    ViewBag.storageData = storageData;
                    ViewBag.qrCodes = qrCodes;
                }
                else
                {
                    ViewBag.storageData = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Inga förvaringsutrymmen tillagda" }
            };
                    ViewBag.QRCodes = new Dictionary<string, string>();
                }
            }

            var clothingItem = new ClothingItem();
            return View(clothingItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClothingItem model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (userId != null)
            {
                model.UserId = userId;
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (model.ClothingImageFile != null && model.ClothingImageFile.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await model.ClothingImageFile.CopyToAsync(memoryStream);


                                //Gör om bytearrayen till en base64sträng
                                var base64String = Convert.ToBase64String(memoryStream.ToArray());

                                model.ClothingImage = base64String;
                            }
                        } else
                        {
                            //Sätt ClothingImage till null om ingen bild läggs in
                            model.ClothingImage = null;
                        }
                        model.ClothingBrand = ToPascalCase(model.ClothingBrand);
                        model.ClothingName = model.ClothingName.Trim();
                        model.ClothingUserId = model.ClothingUserId;
                        model.ClothingSize = model.ClothingSize;
                        model.ClothingColor = ToPascalCase(model.ClothingColor);
                        model.ClothingMaterial = ToPascalCase(model.ClothingMaterial);
                        model.Season = ToPascalCase(model.Season);
                        model.TypeOfClothing = ToPascalCase(model.TypeOfClothing);
                        model.StorageId = model.StorageId;
                        model.UserId = userId;

                        _context.ClothingItem.Add(model);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving ClothingItem: {ex.Message}");
                    }

                }
                else
                {
                    foreach (var modelStateEntry in ModelState.Values)
                    {
                        foreach (var error in modelStateEntry.Errors)
                        {
                            Console.WriteLine($"Error: {error.ErrorMessage}");
                        }
                    }
                } 

                }
            else
            {
                return BadRequest("UserId saknas.");
            }

   
            
                return View(model);
        }

        //Metod för att göra om det användaren lägger in till Pascal Case
        public static string ToPascalCase(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return input ?? string.Empty;

            // Dela upp strängen i ord
            string[] words = input.Split(new char[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);

            // Gör första bokstaven i varje ord stor
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            // Sätt ihop orden igen med bevarade mellanrum
            return string.Join(" ", words);
        }

        //Get ClothingItem Edit
        public async Task<IActionResult> Edit(int ClothingId)
        {

            string? currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId == null)
            {
                _logger.LogWarning("User is not authorized.");
            return Forbid(); // Return 403 Forbidden status if the user is not authorized.
            } 

            

            if (!string.IsNullOrEmpty(currentUserId))
            {
                var familyMemberNames = _context.FamilyMember
                    .Where(fm => fm.UserId == currentUserId)
                    .Select(fm => new SelectListItem
                    {
                        Value = fm.Id.ToString(),
                        Text = fm.Name
                    })
                    .Distinct()
                    .ToList();

                //Kolla om ClothingUserId är satt till -1 och lägg isf till "Ingen vald"
                bool includeNobodyChosen = _context.ClothingItem.Any(c => c.UserId == currentUserId && c.ClothingUserId == -1);

              
                if (familyMemberNames.Any())
                {
                    ViewBag.FamilyMemberNames = new SelectList(familyMemberNames, "Value", "Text");
                }
                else
                {
                    ViewBag.FamilyMemberNames = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Inga medlemmar i hushållet tillagda" }
            };
                }


            }

            if (!string.IsNullOrEmpty(currentUserId))
            {
                var storageNames = _context.GetStorageNameByUserId(currentUserId)
                    .Where(fm => fm.UserId == currentUserId)
                    .GroupBy(fm => fm.StorageName)
                    .Select(group => group.First())
                    .Select(fm => new SelectListItem
                    {
                        Value = fm.StorageId.ToString(),
                        Text = fm.StorageName
                    })
                    .Distinct()
                    .ToList();

                if (storageNames.Any())
                {
                    ViewBag.storageNames = new SelectList(storageNames, "Value", "Text");
                }
                else
                {
                    ViewBag.storageNames = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Inga förvaringsutrymmen tillagda" }
            };
                }
            }

            var clothingItem = await _context.GetClothingItemAsync(ClothingId, currentUserId);

            if (clothingItem == null || !clothingItem.Any())
            {
                Console.WriteLine("Klädesplagg saknas");
            }

            return View(clothingItem);
        }

        //POST Clothing/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ClothingId, 
            //[Bind("ClothingName, ClothingId, ClothingUserId, ClothingBrand, ClothingSize, ClothingColor, Season, TypeOfClothing, StorageId")] 
        ClothingItem model)
        {
            if (ClothingId != model.ClothingId)
            {
                return NotFound();
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("UserId saknas");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        //Hämta nuvarande klädobjekt från databasen

                        var existingClothingItem = await _context.ClothingItem.FindAsync(ClothingId);

                        if (existingClothingItem == null)
                        {
                            return NotFound();
                        }



                        //Uppdaterar det som användaren skrivit in
                        existingClothingItem.ClothingName = model.ClothingName;
                        existingClothingItem.ClothingUserId = model.ClothingUserId;
                        existingClothingItem.ClothingBrand = model.ClothingBrand;
                        existingClothingItem.ClothingColor = model.ClothingColor;
                        existingClothingItem.ClothingSize = model.ClothingSize;
                        existingClothingItem.Season = model.Season;
                        existingClothingItem.TypeOfClothing = model.TypeOfClothing;
                        existingClothingItem.StorageId = model.StorageId;

                        //Spara ändringar till databas
                        _context.ClothingItem.Update(existingClothingItem);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, $"Error updating ClothingItem with ID {ClothingId}. Database update error.");
                        return View(model);
                    }
                }
                return View(model);

            }
            catch 
            {
                return View(model);
            }

        }

        //Get delete view of ClothingItem
        [Authorize]
        public async Task<IActionResult> Delete(int ClothingId)
        {
            string? currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId == null)
            {
                _logger.LogWarning("User is not authorized.");
                return Forbid(); // Return 403 Forbidden status if the user is not authorized.
            }

            var clothingItems = await _context.GetClothingItemAsync(ClothingId, currentUserId);

            if (clothingItems == null)
            {
                Console.WriteLine("Förvaringsutrymme saknas");
            }
            var clothingItem = clothingItems?.First();


            return View("Delete", clothingItem);
        }
        //Delete ClothingItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ClothingId)
        {
            if (ClothingId == 0)
            {
                return NotFound();
            }
            string? currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId == null)
            {
                _logger.LogWarning("User is not authorized.");
                return Forbid(); // Return 403 Forbidden status if the user is not authorized.
            }

            var clothingItem = await _context.ClothingItem.FindAsync(ClothingId);

            if (clothingItem == null)
            {
                return NotFound();
            }
            try
            {
                await _context.DeleteClothingItemByIdAsync(ClothingId, currentUserId);
                TempData["DeleteSuccessMessage"] = "Ditt klädesplagg raderades.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting ClothingItem with ID {ClothingId}. Database update error.");
                TempData["DeleteErrorMessage"] = "Det gick inte att radera ditt klädesplagg.";
                return View(clothingItem);
            }
            return RedirectToAction("Index");

        }

    }
}