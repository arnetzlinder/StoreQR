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

namespace StoreQR.Controllers
{
    public class ClothingController : Controller
    {
        private readonly ILogger<ClothingController> _logger;
        private readonly ApplicationDbContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;

        public ClothingController(ILogger<ClothingController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            //_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        //[Authorize]
        public IActionResult Index()
        {
            var combinedData = _context.GetFamilyMembersForDropdown();
            return View(combinedData);
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