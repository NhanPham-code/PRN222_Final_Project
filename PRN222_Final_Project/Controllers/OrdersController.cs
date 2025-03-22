using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using BLL.Interfaces;
using BLL.Services;
using Newtonsoft.Json;
using MailKit.Search;
using Org.BouncyCastle.Asn1.X509;
using Microsoft.AspNetCore.Authorization;

namespace PRN222_Final_Project.Controllers
{
    public class OrdersController : Controller
    {
        private ICrudRepo<Order, int> _orderRepo;
        private readonly OrderService _orderService;
        private ICrudRepo<User, int> _userRepo;
        private readonly UserService _userSerivce;
        private readonly CartService _cartService;
        private readonly ProductService _productService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ICrudRepo<Order, int> orderRepo, OrderService orderService,
            ILogger<OrdersController> logger, UserService userService, ICrudRepo<User, int> userRepo,
            CartService cartService, ProductService productService)
        {
            _orderRepo = orderRepo;
            _orderService = orderService;
            _logger = logger;
            _userSerivce = userService;
            _userRepo = userRepo;
            _cartService = cartService;
            _productService = productService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepo.GetAllWithInclude(o => o.OrderDetails, o => o.User);
            var orderList = orders.ToList();
            if (orderList != null)
            {
                ViewBag.OrderList = orderList;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(Order order, string shipping_address, string orderData, decimal totalPrice)
        {
            // Get userId ben session
            var userId = getUserID();
            var cartId = getCartList();
            var selectedItems = (await _cartService.GetCartByCartIds(cartId));
            var total = totalPrice;

            /*_logger.LogInformation($"UserId: {userId}");*/
            // Get user by id
            if (userId == 0)
            {
                // Chuyển hướng đến trang đăng nhập nếu không tìm thấy userId
                return RedirectToAction("Login", "Common");
            }

            var orders = await _orderService.CreateOrder(shipping_address, userId, selectedItems, totalPrice);
            return RedirectToAction("Index", "Home"); //Về trang chủ
        }


        [HttpPost]
        public IActionResult Confirm(string name, string phone, string address, string note)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = getUserID();
            var orders = await _orderService.GetOrdersByUserIDWithInclude(userId);
            return View(orders.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> HistoryDetail(int id)
        {
            /*var orders = await _orderRepo.GetByIdWithInclude(id, "OrderId", o => o.User, o => o.OrderDetails);*/

            var orderDetails = await _orderService.GetOrderDetailByOrderId(id);

            return View(orderDetails.ToList());
        }

        public List<int> getCartList()
        {
            var cartIdsJson = TempData["SelectedCartIds"] as string;
            var cartIds = JsonConvert.DeserializeObject<List<int>>(cartIdsJson);

            return cartIds;
        }

        public decimal GetTotalPrice()
        {
            return TempData["TotalPrice"] != null ? Convert.ToDecimal(TempData["TotalPrice"]) : 0m;
        }

        public int getUserID()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }
    }
}
