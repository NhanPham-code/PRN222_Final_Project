using DataAccess.DAOs;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BLL.Services
{
    public class OrderService
    {
        private ICrudRepo<Order, int> _orderRepo;
        private ICrudRepo<User, int> _userRepo;
        private ICrudRepo<Product, int> _productRepo;
        private ICrudRepo<OrderDetail, int> _orderDetailRepo;
        private readonly CartService _cartService;

        List<string> errorMessages = new List<string>();


        public OrderService(ICrudRepo<Order, int> oderRepo, ICrudRepo<User, int> userRepo, 
            ICrudRepo<OrderDetail, int> orderDetailRepo, ICrudRepo<Product, int> productRepo,
            CartService cartService)
        {
            _orderRepo = oderRepo;
            _userRepo = userRepo;
            _orderDetailRepo = orderDetailRepo;
            _productRepo = productRepo;
            _cartService = cartService;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIDWithInclude(int userId)
        {
            var orders = await _orderRepo.GetAllWithInclude(o => o.OrderDetails, o => o.User);
            return orders.Where(o => o.UserId == userId);
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailByOrderId(int orderId)
        {
            var orderDetails = await _orderDetailRepo.GetAllWithInclude(o => o.Order, o => o.Product);
            return orderDetails.Where(o => o.OrderId == orderId);
        }

/*        public async Task<Order?> GetOrderByIdWithInclude(int orderId)
        {
            var order = await _orderRepo.GetByIdWithInclude(orderId, "OrderId", o => o.OrderDetails);
            return order;
        }*/


        public async Task<Order> CreateOrder(string address, int userID, IEnumerable<Cart> selectedItems, decimal total)
        {
            bool isStockAvailable = await CheckStockAvailability(selectedItems);
            if (!isStockAvailable)
            {
                return null;
            }

            //create order
            Order newOrder = new Order
            {
                UserId = userID,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                ShippingAddress = address,
                OrderStatus = "Processing",
                PaymentMethod = "Cash on Delivery",
                PaymentStatus = "Pending",
                User = await _userRepo.GetByIdWithInclude(userID, "UserId"),
            };

            // Lưu user vào database
            await _orderRepo.Add(newOrder);

            //create order details
            foreach (Cart cart in selectedItems)
            {
                OrderDetail newOrderDetails = new OrderDetail
                {
                    OrderId = newOrder.OrderId,
                    ProductId = cart.ProductId,
                    Quantity = cart.Quantity,
                    UnitPrice = cart.Product.Price,
                    Product = cart.Product,
                    Order = newOrder,

                };

                await _orderDetailRepo.Add(newOrderDetails);

                //update remaining
                cart.Product.StockQuantity -= cart.Quantity;
                await _productRepo.Update(cart.Product);

                //remove product from cart
                await _cartService.RemoveProductFromCart(cart.CartId);
            }

            return newOrder;

            
        }

        public async Task<bool> CheckStockAvailability(IEnumerable<Cart> selectedItems)
        {
            foreach (Cart cart in selectedItems)
            {
                var productInDB = await _productRepo.GetById((int)cart.ProductId);

                if (productInDB.StockQuantity < cart.Quantity)
                {
                    await _cartService.RemoveProductFromCart(cart.CartId);
                    return false; // Nếu có sản phẩm không đủ hàng
                }
            }
            return true; // Tất cả sản phẩm đều đủ hàng
        }
    }
}
