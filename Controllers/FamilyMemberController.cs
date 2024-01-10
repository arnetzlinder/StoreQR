using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;

namespace StoreQR.Controllers
{
    public class FamilyMemberController : Controller
    {
        private readonly ILogger<FamilyMemberController> _logger;
        private readonly ApplicationDbContext _context;

        public FamilyMemberController(ILogger<FamilyMemberController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IActionResult Index()
        {
            var familyMembers = _context.FamilyMember?.ToList() ?? new List<FamilyMember>();
            return View(familyMembers);
        }

    }
}
