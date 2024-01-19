using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;

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
                var familyMembers = _context.FamilyMember
            .Where(fm => fm.UserId == currentUserId)
            .ToList();

                return View(familyMembers);
            }
            return View(viewModel);
            
        }

    }
}
