using BLL.Interfaces;
using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{
    public class CartController : Controller
    {
        private ICrudRepo<User, int> _userRepo;
        private UserService _userSerivce;

        public CartController(ICrudRepo<User, int> userRepo, UserService userSerivce)
        {
            _userRepo = userRepo;
            _userSerivce = userSerivce;
        }


        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                return RedirectToAction("Login", "Common");
            }
            var userInfo = await _userRepo.GetById(userId);

            ViewBag.Name = userInfo.FullName;
            ViewBag.Email = userInfo.Email;
            ViewBag.Address = userInfo.Address;
            ViewBag.Phone = userInfo.PhoneNumber;

            return View();
        }

        
    }
}
