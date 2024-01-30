using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Create (StoringUnit model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if(userId != null)
            {
                model.UserId = userId;
                if(ModelState.IsValid)
                {
                    try
                    {
                        if (model.StorageImageFile != null && model.StorageImageFile.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await model.StorageImageFile.CopyToAsync(memoryStream);
                                //Gör om bytearrayen till en base64sträng
                                var base64String = Convert.ToBase64String(memoryStream.ToArray());

                                model.StorageImage = base64String;
                            }
                        } else
                        {
                            //Sätt StorageImage till null om ingen bild läggs in
                            model.StorageImage = null;
                        }

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

        //Get StoringUnit Edit
        public async Task<IActionResult> Edit (int StorageId)
        {
            string? currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId == null)
            {
                _logger.LogWarning("User is not authorized.");
                return Forbid(); // Return 403 Forbidden status if the user is not authorized.
            }

            var storingUnits = await _context.GetStoringUnitAsync(StorageId, currentUserId);

            if (storingUnits == null || !storingUnits.Any())
            {
                Console.WriteLine("Förvaringsutrymme saknas");
            }
            return View(storingUnits);
        }

        //POST StoringUnit Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (int StorageId, StoringUnit model)
        {
            if (StorageId != model.StorageId)
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
                if(ModelState.IsValid)
                {
                    try
                    {
                        //Hämta nuvarande förvaringsutrymmeobjekt från databasen
                        var existingStoringUnit = await _context.StoringUnit.FindAsync(StorageId);

                        if (existingStoringUnit == null)
                        {
                            return NotFound();
                        }

                        //Uppdaterar det som användaren skrivit in
                        existingStoringUnit.StorageName = model.StorageName;
                        existingStoringUnit.StorageDescription = model.StorageDescription;

                        //Spara ändringar till databas
                        _context.StoringUnit.Update(existingStoringUnit);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, $"Error updating StoringUnit with ID {StorageId}. Database update error.");
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
        
        [Authorize]
        public async Task<IActionResult> Delete(int StorageId)
        {
            string? currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId == null)
            {
                _logger.LogWarning("User is not authorized.");
                return Forbid(); // Return 403 Forbidden status if the user is not authorized.
            }

            var storingUnits = await _context.GetStoringUnitAsync(StorageId, currentUserId);

            if (storingUnits == null)
            {
                Console.WriteLine("Förvaringsutrymme saknas");
            }
            var storingUnit = storingUnits?.First();


            return View("Delete", storingUnit);
        }
        //Delete StoringUnit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed (int StorageId)
        {
            if (StorageId == 0)
            {
                return NotFound();
            }
            string? currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId == null)
            {
                _logger.LogWarning("User is not authorized.");
                return Forbid(); // Return 403 Forbidden status if the user is not authorized.
            }

            var storingUnit = await _context.StoringUnit.FindAsync(StorageId);

            if (storingUnit == null)
            {
                return NotFound();
            }
            try
            {
                await _context.DeleteStorageUnitByIdAsync(StorageId, currentUserId);
                TempData["DeleteSuccessMessage"] = "Dtt förvaringsutrymme raderades.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting StoingUnit with ID {StorageId}. Database update error.");
                TempData["DeleteErrorMessage"] = "Det gick inte att radera ditt förvaringsutrymme.";
                return View(storingUnit);
            }
            return RedirectToAction("Index");

        }
    }
}
