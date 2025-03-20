using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.SignalR;
using Razor_UI.SignalRHub;

namespace Razor_UI.Pages.Admin
{
    public class IndexModel : PageModel
    {

        private ICrudRepo<Feedback, int> _feedbackRepo;
        private readonly IHubContext<DataSignalR> _hubContext;
        public IndexModel(ICrudRepo<Feedback, int> feedbackRepo, IHubContext<DataSignalR> hubContext)
        {
            this._feedbackRepo = feedbackRepo;
            this._hubContext = hubContext;
        }
        public IList<Feedback> Feedback { get; set; } = new List<Feedback>();

        // lấy string search
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        public async Task OnGetAsync()
        {
            Feedback = (await _feedbackRepo.GetAllWithInclude(f => f.User)).ToList();
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                // Kiểm tra nếu `SearchQuery` có thể parse sang DateOnly
                if (DateOnly.TryParse(SearchQuery, out var searchDate))
                {
                    Feedback = Feedback.Where(f => f.SubmittedDate.HasValue && DateOnly.FromDateTime(f.SubmittedDate.Value) == searchDate).ToList();
                }
                else
                {
                    // Tìm theo mô tả feedback hoặc tên User
                    Feedback = Feedback.Where(f =>
                        (!string.IsNullOrEmpty(f.Description) && f.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (f.User != null && !string.IsNullOrEmpty(f.User.FullName) && f.User.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var feedback = await _feedbackRepo.GetById(id);
            if (feedback != null)
            {
                await _feedbackRepo.Delete(feedback);
                await _hubContext.Clients.All.SendAsync("feedback");
            }

            // Load lại danh sách feedback trước khi return
            Feedback = (await _feedbackRepo.GetAllWithInclude(f => f.User)).ToList();

            return RedirectToPage("/Admin/FeedbackManagement");
        }
    }
}
