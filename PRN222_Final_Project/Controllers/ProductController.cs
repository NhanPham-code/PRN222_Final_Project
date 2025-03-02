using BLL.Interfaces;
using BLL.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{
    public class ProductController : Controller
    {
        private ProductService productService;
        private int DEFAULT_CATEGORY_ID_TO_DIPLAY_PRODUCT = 1;

        private ICrudRepo<Category, int> _categoryRepo;

        public ProductController(ProductService productService, ICrudRepo<Category, int> categoryRepo)
        {
            this.productService = productService;
            this._categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index(int categoryId)
        {
            // get category
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            if (categoryId == 0)
            {
                var productsDefault = await productService.GetProductsByCategory(DEFAULT_CATEGORY_ID_TO_DIPLAY_PRODUCT);
                return View(productsDefault.ToList());
            }

            var products = await productService.GetProductsByCategory(categoryId);
            return View(products.ToList());
        }
    }
}
