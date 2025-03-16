using DataAccess.DAOs;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using System.Runtime.CompilerServices;

namespace BLL.Services
{
    public class OrderService
    {
        private ICrudRepo<Order, int> _orderRepo;
        private ICrudRepo<User, int> _userRepo;

        public OrderService(ICrudRepo<Order, int> oderRepo, ICrudRepo<User, int> userRepo)
        {
            _orderRepo = oderRepo;
            _userRepo = userRepo;
        }

        public async Task<Order> CreateOrder(string address, int userID)
        {
            Order newOrder = new Order
            {
                UserId = userID,
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                ShippingAddress = address,
                OrderStatus = "Processing",
                PaymentMethod = "Cash on Delivery",
                PaymentStatus = "Pending",
                User = await _userRepo.GetByIdWithInclude(userID, "UserId"),
            };

            // Lưu user vào database
            await _orderRepo.Add(newOrder);

            Console.WriteLine($"Order ID: {newOrder.OrderId}, User ID: {newOrder.UserId}, Total: {newOrder.TotalAmount}");

            return newOrder;
        }
    }
}
