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
        private ICrudRepo<User, int> _userRepo;

        private EmailService emailService;

        public CommonController(ICrudRepo<Category, int> categoryRepo, ICrudRepo<User, int> userRepo, EmailService emailService)
        {
            _categoryRepo = categoryRepo;
            _userRepo = userRepo;
            this.emailService = emailService;
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

            // check user exit (tach service)
            var users = await _userRepo.GetAllWithInclude(u => u.Carts, u => u.Orders);
            var user = users.FirstOrDefault(u => string.Equals(u.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Kiểm tra mật khẩu
            if (!VerifyPassword(password, user.PasswordSalt, user.PasswordHash))
            {
                ViewBag.ErrorLogin = "Wrong email or password.";
                Console.WriteLine(ViewBag.ErrorLogin);
                return View("Login");
            }

            // Lưu thông tin user vào Session
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // Lấy list product trong cart của user

            return RedirectToAction("Index", "Home");
        }

        // Kiểm tra mật khẩu
        private bool VerifyPassword(string password, byte[] storedSalt, byte[] storedHash)
        {
            using (var hmac = new HMACSHA512(storedSalt)) // Dùng lại storedSalt để tạo HMACSHA512
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Tạo hash mới từ password nhập vào

                // So sánh toàn bộ 64 byte
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private void HashPassword(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key; // Tạo salt (128 byte)
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Băm ra 64 byte
            }
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

            // check user exit (tach service)
            var users = await _userRepo.GetAllWithInclude(u => u.Carts, u => u.Orders);
            var user = users.FirstOrDefault(u => string.Equals(u.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));

            if (user != null)
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

            bool emailSent = await emailService.SendEmailAsync(email, "Verify Your Account", emailBody);
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

            // Mã hóa mật khẩu
            byte[] passwordSalt, passwordHash;
            HashPassword(password, out passwordSalt, out passwordHash);

            // Tạo user mới
            User newUser = new User
            {
                FullName = fullname,
                Email = email,
                Address = address,
                PhoneNumber = phone,
                RegistrationDate = DateTime.Now,
                Role = "Customer",
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash
            };

            // Lưu user vào database
            await _userRepo.Add(newUser);

            return RedirectToAction("Login", "Common");
        }

    }
}
