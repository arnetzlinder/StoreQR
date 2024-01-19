using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;

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
            string? currentUserId = _userManager.GetUserId(HttpContext.User);
            if (currentUserId != null)
            {
                var storageUnits = _context.StoringUnit
                    .Where(s => s.UserId == currentUserId)
                    .ToList();

                return View(storageUnits);
            }
           return View(viewModel);
        }
    }
}
