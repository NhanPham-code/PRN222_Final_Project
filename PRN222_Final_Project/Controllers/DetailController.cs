using BLL.Interfaces;
using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{
    public class DetailController : Controller
    {
        private ProductService productService;
        private ICrudRepo<Category, int> _categoryRepo;

        public DetailController(ProductService productService, ICrudRepo<Category, int> _categoryRepo)
        {
            this.productService = productService;
            this._categoryRepo = _categoryRepo;
        }

        public async Task<IActionResult> Index(int productId)
        {
            // get category in header
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            // get product to show detail
            var product = await productService.GetProductById(productId);

            // get similar products by categoryId of product detail
            var similarProducts = await productService.GetProductsByCategory(product.CategoryId ?? 0);
            similarProducts.Remove(product);

            ViewBag.Product = product;
            ViewBag.SimilarProducts = similarProducts;
            return View();
        }
    }
}
