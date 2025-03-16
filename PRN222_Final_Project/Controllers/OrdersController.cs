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

namespace PRN222_Final_Project.Controllers
{
    public class OrdersController : Controller
    {
        private ICrudRepo<Order, int> _orderRepo;
        private OrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ICrudRepo<Order, int> orderRepo, OrderService orderService, ILogger<OrdersController> logger)
        {
            _orderRepo = orderRepo;
            _orderService = orderService;
            _logger = logger;
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
        public async Task<IActionResult> CreateOrder(Order order, string shipping_address, string orderData)
        {
            // Get userId ben session
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            /*_logger.LogInformation($"UserId: {userId}");*/
            // Get user by id
            if (userId == 0)
            {
                // Chuyển hướng đến trang đăng nhập nếu không tìm thấy userId
                return RedirectToAction("Login", "Common");
            }

            var orders = await _orderService.CreateOrder(shipping_address, userId);
            return RedirectToAction("Index", "Home"); //Về trang chủ
        }


        [HttpPost]
        public IActionResult Confirm(string name, string phone, string address, string note)
        {
            return View();
        }
    }
}
