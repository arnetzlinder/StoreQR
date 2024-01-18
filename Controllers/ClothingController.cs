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
using StoreQR.Interface;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public IActionResult Index(ClothingViewModel viewModel, bool? ResetFilters)
        {
            //Hämta användarens id med hjälp av user manager
            string? currentUserId = _userManager.GetUserId(HttpContext.User);
            if (currentUserId != null)
            {
                //Hämtar alla värden först
                var familyMembersAndStorage = _context.CombineFamilyNameAndStorageNameByUserId(currentUserId);

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
                        FamilyMemberName = c.FamilyMemberName
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
                ViewBag.DistinctBrands = distinctFamilyMembers.Select(c => c.ClothingBrand).Distinct().ToList();
                ViewBag.DistinctSizes = distinctFamilyMembers.Select(c => c.ClothingSize).Distinct().ToList();
                ViewBag.DistinctColors = distinctFamilyMembers.Select(c => c.ClothingColor).Distinct().ToList();
                ViewBag.DistinctSeasons = distinctFamilyMembers.Select(c => c.Season).Distinct().ToList();
                ViewBag.DistinctMaterials = distinctFamilyMembers.Select(c => c.ClothingMaterial).Distinct().ToList();
                ViewBag.DistinctTypesOfClothing = distinctFamilyMembers.Select(c => c.TypeOfClothing).Distinct().ToList();
                ViewBag.DistinctFamilyMemberName = distinctFamilyMembers.Select(c => c.FamilyMemberName).Distinct().ToList();
                ViewBag.StorageUnitName = storageUnits.Select(c => c.StorageName).Distinct().ToList();

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
                    return View(allClothingItems);
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



                    return View(filteredClothingItems);
                }
            } else
            {
                Console.WriteLine("Something went wrong");
                throw new Exception("Something went wrong");
            }
           
          
        }
        static List<string> ConvertToPascalCase(List<string> inputList) 
        { 
            List<string> pascalCaseList = new List<string>(); 
            foreach (string input in inputList) 
            { 
                string[] words = input.Split(' '); for (int i = 0; i < words.Length; i++) 
                {
                    pascalCaseList.Add(char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower()); 
                } 
            } 
            return pascalCaseList; 
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

                        //if (image != null && image.Length > 0)
                        //{
                        //    using (var memoryStream = new MemoryStream())
                        //    {
                        //        await image.CopyToAsync(memoryStream);
                        //        model.ClothingImage = memoryStream.ToArray();
                        //    }
                        //}

                        model.ClothingBrand = model.ClothingBrand.Trim();
                        model.ClothingName = model.ClothingName.Trim();
                        model.ClothingUserId = model.ClothingUserId;
                        model.ClothingSize = model.ClothingSize.Trim();
                        model.ClothingColor = model.ClothingColor.Trim();
                        model.ClothingMaterial = model.ClothingMaterial.Trim();
                        model.Season = model.Season.Trim();
                        model.TypeOfClothing = model.TypeOfClothing.Trim();
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
    }
}