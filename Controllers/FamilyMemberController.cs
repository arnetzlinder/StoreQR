using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                if (familyMembers != null)
                {
                    return View(familyMembers);
                }
            }
            return View(viewModel);
            
        }

        //Hämta create-vy för familjemedlemmar
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        //Skicka create
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

        //Get FamilyMemberEdit
        [Authorize]
        public async Task<IActionResult> Edit (int Id)
        {
            string? currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId == null)
            {
                _logger.LogWarning("User is not authorized.");
                return Forbid(); // Return 403 Forbidden status if the user is not authorized.
            }

            var familyMember = await _context.GetFamilyMemberAsync(Id, currentUserId);
            if (familyMember == null)
                {
                    Console.WriteLine("Familjemedlem saknas");
                    return NotFound();
            }

            return View(familyMember);
        }

        //POST FamilyMember/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (int Id, FamilyMember model)
        {
            if (Id != model.Id)
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
                        //Hämta nuvarande familjemedlemsobjekt

                        var existingFamilyMember = await _context.FamilyMember.FindAsync(Id);

                        if (existingFamilyMember == null)
                        {
                            return NotFound();
                        }

                        //Uppdaterar det som användaren skrivit in
                        existingFamilyMember.Name = model.Name;

                        //Spara ändringar till databas
                        _context.FamilyMember.Update(existingFamilyMember);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, $"Error updating FamilyMember with ID {Id}. Database update error.");
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
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("UserId saknas");
            }

            var FamilyMember = await _context.FamilyMember.FindAsync(id);
            if (FamilyMember == null)
            {
                return NotFound();
            }
            try
            {
                await _context.DeleteFamilyMemberByIdAsync(id, userId);
                TempData["DeleteSuccessMessage"] = "Din hushållsmedlem raderades.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting FamilyMember with ID {id}. Database update error.");
                TempData["DeleteErrorMessage"] = "Det gick inte att radera din hushållsmedlem.";
                return View();
            }

            return RedirectToAction("Index");
        }

    }



}
