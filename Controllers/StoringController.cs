using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;

namespace StoreQR.Controllers
{
    public class StoringController : Controller
    {
        private readonly ILogger<StoringController> _logger;
        private readonly ApplicationDbContext _context;

        public StoringController(ILogger<StoringController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            var storing = _context.StoringUnit.ToList();
            return View(storing);
        }
    }
}
