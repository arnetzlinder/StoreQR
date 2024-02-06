//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using StoreQR.Data;
//using StoreQR.Interface;
//using System.Threading.Tasks;

//namespace StoreQR.Controllers
//{
//    public class DeleteController : Controller
//    {
//        private readonly ILogger<DeleteController> _logger;
//        private readonly DataAccess? _dataAccess;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly SignInManager<IdentityUser> _signInManager;

//        public DeleteController(ILogger<DeleteController> logger,
//          DataAccess _dataAccess,
//          UserManager<IdentityUser> userManager,
//          SignInManager<IdentityUser> signInManager)
//        {
//            _logger = logger;
//            _dataAccess = _dataAccess ?? throw new ArgumentNullException(nameof(_dataAccess));
//            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
//            _signInManager = signInManager;
//        }

//        public IActionResult DeleteUser(string userId)
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ConfirmDeleteUser (string userId)
//        {
//            var success = _dataAccess != null && await _dataAccess.DeleteAllUserInfoByUserId(userId, _userManager, _signInManager);
//            if (success)
//            {
//                return RedirectToAction("Index", "Home");
//            }
//            else
//            {
//                return RedirectToAction("Error");
//            }
            


//        }
//    }
//}
