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

namespace StoreQR.Controllers
{
    public class ClothingController : Controller
    {
        private readonly ILogger<ClothingController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IClothingFilterService _filterService;
        //private readonly UserManager<ApplicationUser> _userManager;

        public ClothingController(ILogger<ClothingController> logger, ApplicationDbContext context, IClothingFilterService filterService)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            //_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        //[Authorize]
        public IActionResult Index(ClothingViewModel viewModel)
        {
            //Hämtar alla värden först
            var distinctValues = _context.ClothingItem
                .Select(c => new
                {
                    ClothingBrand = c.ClothingBrand,
                    ClothingSize = c.ClothingSize,
                    ClothingColor = c.ClothingColor,
                    Season = c.Season,
                    ClothingMaterial = c.ClothingMaterial,
                    TypeOfClothing = c.TypeOfClothing
                })
                .Distinct()
                .ToList();

            ViewBag.DistinctBrands = _context.ClothingItem.Select(c => c.ClothingBrand).Distinct().ToList();
            ViewBag.DistinctSizes = _context.ClothingItem.Select(c => c.ClothingSize).Distinct().ToList();
            ViewBag.DistinctColors = _context.ClothingItem.Select(c => c.ClothingColor).Distinct().ToList();
            ViewBag.DistinctSeasons = _context.ClothingItem.Select(c => c.Season).Distinct().ToList();
            ViewBag.DistinctMaterials = _context.ClothingItem.Select(c => c.ClothingMaterial).Distinct().ToList();
            ViewBag.DistinctTypesOfClothing = _context.ClothingItem.Select(c => c.TypeOfClothing).Distinct().ToList();

            if (string.IsNullOrEmpty(viewModel.ClothingBrand))
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
                              viewModel.TypeOfClothing
                          );

              

                return View(filteredClothingItems);
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
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClothingItem model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.GetUserAsync(User);
                //if (user != null)
                //{
                //    model.UserId = int.Parse(user.Id);
                { 

                    //if (image != null && image.Length > 0)
                    //{
                    //    using (var memoryStream = new MemoryStream())
                    //    {
                    //        await image.CopyToAsync(memoryStream);
                    //        model.ClothingImage = memoryStream.ToArray();
                    //    }
                    //}
                    _context.ClothingItem.Add(model);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
    }
}