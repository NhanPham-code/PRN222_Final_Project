using BLL.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CartService
    {
        private readonly ICrudRepo<Cart, int> _cartRepo;

        public CartService(ICrudRepo<Cart, int> cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public async Task AddToCart(int userId, int productId, int quantity)
        {
            var carts = await _cartRepo.GetAll();
            var existingCart = carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (existingCart != null)
            {
                existingCart.Quantity += quantity;
                await _cartRepo.Update(existingCart);
            }
            else
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.UtcNow
                };
                await _cartRepo.Add(newCart);
            }
        }

        public async Task UpdateQuantity(int cartId, int quantity)
        {
            var cart = await _cartRepo.GetById(cartId);
            if (cart != null)
            {
                cart.Quantity = quantity;
                await _cartRepo.Update(cart);
            }
        }

        public async Task<IEnumerable<Cart>> GetCartByUserId(int userId)
        {
            var carts = await _cartRepo.GetAllWithInclude(c => c.Product);
            return carts.Where(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Cart>> GetCartByCartIds(List<int> cartIds)
        {
            var carts = await _cartRepo.GetAllWithInclude(c => c.Product);
            return carts.Where(c => cartIds.Contains(c.CartId));
        }
        public async Task<bool> RemoveProductFromCart(int cartId)
        {
            var cart = await _cartRepo.GetById(cartId);
            if (cart != null)
            {
                return await _cartRepo.Delete(cart);
            }
            return false;
        }
    }
}

