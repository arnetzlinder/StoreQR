using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;
using System.Security.Claims;

namespace StoreQR.Controllers
{
    public class StoringController : Controller
    {
        private readonly ILogger<StoringController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StoringController(ILogger<StoringController> logger, 
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public IActionResult Index(StoringUnit viewModel)
        {
            //Hämta användarens id med hjälp av user manager
            string? currentUserId = _userManager.GetUserId(HttpContext.User);
            if (currentUserId != null)
            {
                var storageUnits = _context.GetStorageUnitsFilteredByUserId(currentUserId);

                return View(storageUnits);
            }
           return View(viewModel);
        }

        //Hämta create-vy för förvaringsutrymmen
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (StoringUnit model 
            //,IFormFile formFile
            )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if(userId != null)
            {
                model.UserId = userId;
                if(ModelState.IsValid)
                {
                    try
                    {
                        //if(model.StorageImageFile != null && model.StorageImageFile.Length > 0)
                        //{
                        //    using (var memoryStream = new MemoryStream())
                        //    {
                        //        await model.StorageImageFile.CopyToAsync(memoryStream);
                        //        model.StorageImage = memoryStream.ToArray();
                        //    }
                        //}

                        model.StorageName = model.StorageName.Trim();
                        model.StorageDescription = model.StorageDescription.Trim();

                        _context.StoringUnit.Add(model);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving StoringUnit: {ex.Message}");
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
                return BadRequest("UserId saknas");
            }
            return View(model);
        }
    }
}
