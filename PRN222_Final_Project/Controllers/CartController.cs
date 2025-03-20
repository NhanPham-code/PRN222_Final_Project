using BLL.Interfaces;
using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{

    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private ICrudRepo<Category, int> _categoryRepo;
        private ICrudRepo<User, int> _userRepo;

        public CartController(CartService cartService, ICrudRepo<Category, int> categoryRepo, ICrudRepo<User, int> userRepo)
        {
            _cartService = cartService;
            _categoryRepo = categoryRepo;
            _userRepo = userRepo;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Common");
            }
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }


            var cartItems = await _cartService.GetCartByUserId(userId.Value);
            return View(cartItems.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Common");
            }

            await _cartService.AddToCart(userId.Value, productId, quantity);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(List<int> cartID, List<int> quantity)
        {
            if (cartID.Count != quantity.Count)
            {
                return BadRequest("Invalid cart data");
            }

            for (int i = 0; i < cartID.Count; i++)
            {
                await _cartService.UpdateQuantity(cartID[i], quantity[i]);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartID)
        {
            await _cartService.RemoveProductFromCart(cartID);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut(List<int> cartIds)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Common");
            }
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            if (cartIds == null || !cartIds.Any())
            {
                return RedirectToAction("Cart");
            }
            var User = await _userRepo.GetByIdWithInclude(userId, "UserId", u => u.Carts, u => u.Feedbacks, u => u.Orders);
            ViewBag.User = User;

            var selectedItems = (await _cartService.GetCartByCartIds(cartIds)).ToList();

            return View("CheckOut", selectedItems);
        }
    }
}