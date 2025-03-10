using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_UI.Pages.Admin
{
    public class OrderManagementModel : PageModel
    {
        private readonly ICrudRepo<Order, int> _orderRepo;
        private readonly ICrudRepo<User, int> _userRepo;

        [BindProperty(SupportsGet = true)]
        public decimal TotalOrders { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal TotalCancelled { get; set; }
        public OrderManagementModel(ICrudRepo<Order, int> orderRepo, ICrudRepo<User, int> userRepo)
        {
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }


        [BindProperty]

        public List<Order> Orders { get; set; } = new List<Order>();

        [BindProperty]
        public Order order { get; set; }

        public async Task OnGetAsync()
        {
            Orders = (await _orderRepo.GetAllWithInclude(o => o.User, o => o.OrderDetails)).ToList(); // Lấy danh sách Order từ repository

            TotalCancelled = Orders.Where(o => o.OrderStatus == "Cancelled").Sum(o => o.TotalAmount); //Total of cancelled

            TotalOrders = Orders.Sum(o => o.TotalAmount); //Total of orders

            ViewData["TotalDelivered"] = Orders.Where(o => o.OrderStatus == "Delivered").Sum(o => o.TotalAmount);
            ViewData["TotalProcessing"] = Orders.Where(o => o.OrderStatus == "Processing").Sum(o => o.TotalAmount);
        }
    }
}
