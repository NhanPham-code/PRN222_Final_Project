using BLL.Interfaces;
using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace PRN222_Final_Project.Controllers
{
    public class CommonController : Controller
    {
        private ICrudRepo<Category, int> _categoryRepo;

        private UserService _userService;
        private EmailService _emailService;

        public CommonController(ICrudRepo<Category, int> categoryRepo, UserService userService, EmailService emailService)
        {
            _categoryRepo = categoryRepo;
            this._userService = userService;
            this._emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Forgot()
        {
            // get category in header
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            return View("Forgot");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // get category in header
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter both email and password.";
                return View();
            }

            var user = await _userService.Login(email, password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View("Login");
            }

            // Lưu thông tin user vào Session
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // Lấy list product trong cart của user

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            // get category in header
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullname, string email, string address, string phone, string password)
        {
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address)
            || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please fill all fields";
                return View();
            }

            // check user exit
            bool userExits = await _userService.CheckUserExits(email);
            if (userExits)
            {
                ViewBag.Error = "Email has already been registered";
                return View();
            }

            // Lưu thông tin vào TempData để dùng trong Verify
            TempData["Fullname"] = fullname;
            TempData["Email"] = email;
            TempData["Address"] = address;
            TempData["Phone"] = phone;
            TempData["Password"] = password;

            // send email
            string verifyCode = new Random().Next(100000, 999999).ToString(); // Mã xác nhận 6 số
            string emailBody = $"Your verification code is: <b>{verifyCode}</b>";

            bool emailSent = await _emailService.SendEmailAsync(email, "Verify Your Account", emailBody);
            if (!emailSent)
            {
                Console.WriteLine("Bug Verify");
            }

            // Chuyển hướng verigy sau khi đăng ký thành công
            return RedirectToAction("Verify", "Common");
        }

        [HttpGet]
        public async Task<IActionResult> Verify()
        {
            // get category in header
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify(string verifyCode)
        {
            // Lấy thông tin đăng ký từ TempData
            string fullname = TempData["Fullname"] as string;
            string email = TempData["Email"] as string;
            string address = TempData["Address"] as string;
            string phone = TempData["Phone"] as string;
            string password = TempData["Password"] as string;

            var newUser = await _userService.Register(fullname, email, address, phone, password);

            return RedirectToAction("Login", "Common");
        }

    }
}
