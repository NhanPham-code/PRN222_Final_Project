using BLL.Interfaces;
using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
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

        // ********* Login Handle *********
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

            if (user == null || user.Role != "Customer")
            {
                ViewBag.Error = "Invalid email or password.";
                return View("Login");
            }

            // Lưu thông tin user vào Session
            HttpContext.Session.SetInt32("UserId", user.UserId);

            return RedirectToAction("Index", "Home");
        }



        // ********* Logout handle *********
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



        // ********* Register Handle *********
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

            // create verify code and send email
            string verifyCode = new Random().Next(100000, 999999).ToString(); // Mã xác nhận 6 số
            string emailBody = $"Your verification code is: <b>{verifyCode}</b>";

            bool emailSent = await _emailService.SendEmailAsync(email, "Verify Your Account", emailBody);
            if (!emailSent)
            {
                Console.WriteLine("Bug Verify");
            }

            // Save verify code to session
            HttpContext.Session.SetString("VerifyCode", verifyCode);

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
            // Get data from TempData
            string fullname = TempData["Fullname"] as string;
            string email = TempData["Email"] as string;
            string address = TempData["Address"] as string;
            string phone = TempData["Phone"] as string;
            string password = TempData["Password"] as string;

            // get verify code from session
            string storedCode = HttpContext.Session.GetString("VerifyCode"); // correct code

            if (string.IsNullOrEmpty(storedCode) || verifyCode != storedCode)
            {
                ViewBag.ErrorMessage = "Invalid Verify Code!";
                return View();
            }

            // add new user
            var newUser = await _userService.Register(fullname, email, address, phone, password);
            return RedirectToAction("Login", "Common");
        }



        // ********* Forgot Password Handle  ***********
        
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

        [HttpPost]
        public async Task<IActionResult> SendResetCode()
        {
            // get email from Form send by Ajax
            string email = Request.Form["email"];

            // check user exits by email
            bool userExits = await _userService.CheckUserExits(email);
            if (userExits == false)
            {
                return Content("Email không tồn tại!");
            }

            // create verify code and send email
            string verifyCode = new Random().Next(100000, 999999).ToString(); // Mã xác nhận 6 số
            string emailBody = $"Your verification code is: <b>{verifyCode}</b>";

            bool emailSent = await _emailService.SendEmailAsync(email, "Verify Your Account", emailBody);
            if (!emailSent)
            {
                Console.WriteLine("Bug Verify");
            }

            // Save verify code to session
            HttpContext.Session.SetString("VerifyCode", verifyCode);
            // Save email to session to update new password
            HttpContext.Session.SetString("Email", email);

            return Content("Mã xác nhận đã được gửi!");
        }

        [HttpPost]
        public IActionResult VerifyResetCode()
        {
            string code = Request.Form["code"]; // code from user input 

            string storedCode = HttpContext.Session.GetString("VerifyCode"); // correct code

            if (string.IsNullOrEmpty(storedCode) || code != storedCode)
            {
                return Content("Mã xác nhận không hợp lệ!");
            }

            return Content("Mã xác nhận chính xác, vui lòng nhập mật khẩu mới!");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword()
        {
            string newPassword = Request.Form["newPassword"]; // string new pass from user input

            string email = HttpContext.Session.GetString("Email"); // email of user to update new pass

            // call user Service to update new password for this email
            bool result = await _userService.ResetPasswordByEmail(email, newPassword);

            if(result == false)
            {
                return Content("Lỗi hệ thống!");
            }

            return Content("Mật khẩu đã được cập nhật!");
        }


        // ********* Profile Handle *********
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // get category in header
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            var user = await _userService.GetUserById(userId.Value);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            return View("Profile", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string email, string phone, string address)
        {
            // get category in header
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            var user = await _userService.GetUserById(userId.Value);

            if(user != null)
            {
                user.Email = email;
                user.PhoneNumber = phone;
                user.Address = address;
                await _userService.UpdateUser(user);
                ViewData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            }
            return View("Profile", user); // truyền theo user mới hiển thị lên view
        }
    }
}
