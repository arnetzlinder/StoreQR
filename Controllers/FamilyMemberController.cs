using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;
using System;
using System.Security.Claims;

namespace StoreQR.Controllers
{
    public class FamilyMemberController : Controller
    {
        private readonly ILogger<FamilyMemberController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FamilyMemberController(ILogger<FamilyMemberController> logger, 
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public IActionResult Index(FamilyMember viewModel)
        {
            //Hämta användarens id med hjälp av user manager
            string? currentUserId = _userManager.GetUserId(HttpContext.User);
            if (currentUserId != null)
            {
               //Hämtar alla användarens hushållsmedlemmar
               var familyMembers = _context.GetFamilyMembersFilteredByUserId(currentUserId);

                return View(familyMembers);
            }
            return View(viewModel);
            
        }

        //Hämta create-vy för familjemedlemmar
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FamilyMember model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (userId != null)
            {
                model.UserId = userId;
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.Name = model.Name.Trim();
                        model.UserId = userId;

                        _context.FamilyMember.Add(model);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving FamilyMember: {ex.Message}");
                    }
                }
                else 
                { 
                    return View(model); 
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
