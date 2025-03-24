using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Razor_UI.SignalRHub;

namespace Razor_UI.Pages.Admin
{
    public class AccountManagementModel : PageModel
    {
        private readonly UserService _userService;
        private readonly IHubContext<DataSignalR> _hubContext;

        public AccountManagementModel(UserService userService, IHubContext<DataSignalR> hubContext)
        {
            this._userService = userService;
            this._hubContext = hubContext;
        }

        public string Message { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public List<User> UserList { get; set; } = new List<User>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userList = await _userService.GetAllUser();

            if (userList == null)
            {
                Message = "No accounts";
                return Page();
            }

            UserList = userList.ToList();
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int userId)
        {
            // delete user account
            bool result = await _userService.DeleteUser(userId);

            // get new list account after delete or not
            var userList = await _userService.GetAllUser();
            UserList = userList.ToList();

            if (result)
            {
                await _hubContext.Clients.All.SendAsync("DeleteUser");
                Message = "Delete account successfully!";
                
            } else
            {
                Message = "This account can not be deleted. Because it has relative information such as: orders, feedbacks";
            }

            return Page();
        }

        [BindProperty]
        public string SearchKey { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostSearchAsync()
        {
            var searchListResult = await _userService.Search(SearchKey);

            if(searchListResult == null)
            {
                Message = "No accounts";
                return Page();
            }

            UserList = searchListResult.ToList();
            return Page();
        }
    }
}
