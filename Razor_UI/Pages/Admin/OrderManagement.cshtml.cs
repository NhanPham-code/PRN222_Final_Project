using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Razor_UI.SignalRHub;

namespace Razor_UI.Pages.Admin
{
    public class OrderManagementModel : PageModel
    {
        private readonly ICrudRepo<Order, int> _orderRepo;
        private readonly ICrudRepo<User, int> _userRepo;
        private readonly IHubContext<DataSignalR> _hubContext;

        [BindProperty(SupportsGet = true)]
        public decimal TotalOrders { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal TotalCancelled { get; set; }

        public List<Order> listPaymentMethod {  get; set; }
        public OrderManagementModel(ICrudRepo<Order, int> orderRepo, ICrudRepo<User, int> userRepo, IHubContext<DataSignalR> hubContext)
        {
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _hubContext = hubContext;
        }


        [BindProperty]

        public List<Order> Orders { get; set; } = new List<Order>();

        [BindProperty]
        public Order order { get; set; }

        public async Task OnGetAsync()
        {
            Orders = (await _orderRepo.GetAllWithInclude(o => o.User, o => o.OrderDetails)).ToList(); // Lấy danh sách Order từ repository

            ViewData["PaymentStatus"] = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pending", Text = "Pending" },
                new SelectListItem { Value = "Paid", Text = "Paid" }
            };


            TotalCancelled = Orders.Where(o => o.OrderStatus == "Cancelled").Sum(o => o.TotalAmount); //Total of cancelled

            TotalOrders = Orders.Sum(o => o.TotalAmount); //Total of orders

            ViewData["TotalDelivered"] = Orders.Where(o => o.OrderStatus == "Delivered").Sum(o => o.TotalAmount);
            ViewData["TotalProcessing"] = Orders.Where(o => o.OrderStatus == "Processing").Sum(o => o.TotalAmount);
        }

        public async Task<IActionResult> OnPostAsync(int OrderId, string PaymentStatus, string OrderStatus)
        {
            var order = await _orderRepo.GetByIdWithInclude(OrderId, "OrderId", o => o.User, o => o.OrderDetails);
            if (order == null)
            {
                return NotFound();
            }

            order.PaymentStatus = PaymentStatus;
            order.OrderStatus = OrderStatus;
            await _orderRepo.Update(order);

            if (!string.IsNullOrEmpty(PaymentStatus))
            {
                await _hubContext.Clients.All.SendAsync("ReceivePaymentUpdate", OrderId, PaymentStatus);
            }

            if (!string.IsNullOrEmpty(OrderStatus))
            {
                await _hubContext.Clients.All.SendAsync("ReceiveOrderStatusUpdate", OrderId, OrderStatus);
            }

            return RedirectToPage();
        }
    }
}
