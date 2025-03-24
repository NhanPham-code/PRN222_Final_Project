using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_UI.Pages
{
    public class IndexModel : PageModel
    {

        private readonly UserService _userService;

        public IndexModel(UserService userService)
        {
            this._userService = userService;
        }


        [BindProperty]
        public User Admin { get; set; } = new User();
        public string ErrorMessage { get; set; } = string.Empty;


        public void OnGet()
        {
            ErrorMessage = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) 
            {
                // Hiển thị lỗi nếu đăng nhập thất bại
                ErrorMessage = "Please enter both email and password!";
            }

            var admin = await _userService.Login(email, password);

            if (admin == null || admin.Role != "Admin")
            {
                // Hiển thị lỗi nếu đăng nhập thất bại
                ErrorMessage = "Invalid email or password!";
                return Page();
            } else
            {
                // Lưu thông tin user vào Session
                HttpContext.Session.SetInt32("AdminId", admin.UserId);
            }

            return RedirectToPage("/Admin/Home");
        }
    }
}
