using Microsoft.AspNetCore.Mvc;
using StoreQR.Data;
using StoreQR.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace StoreQR.Controllers
{
    public class ClothingController : Controller
    {
        private readonly ILogger<ClothingController> _logger;
        private readonly ApplicationDbContext _context;

        public ClothingController(ILogger<ClothingController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Authorize]
        public IActionResult Index()
        {
            var clothes = _context.ClothingItem.ToList();
            return View(clothes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}