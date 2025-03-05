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

            // get product by default category
            if (categoryId == 0)
            {
                ViewBag.Category = "All Products";
                var productsDefault = await productService.GetProductsByCategory(categoryId);
                return View(productsDefault.ToList());
            }

            // get products by category
            var products = await productService.GetProductsByCategory(categoryId);
            return View(products.ToList());
        }

        public async Task<IActionResult> Search(string searchKey)
        {
            // Lấy danh sách danh mục để hiển thị trong ViewBag
            var categories = (await _categoryRepo.GetAll()).ToList();
            ViewBag.Categories = categories;

            // Nếu searchKey rỗng, lấy sản phẩm mặc định
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                var productsDefault = await productService.GetProductsByCategory(DEFAULT_CATEGORY_ID_TO_DIPLAY_PRODUCT);
                return View("Index", productsDefault.ToList());
            }

            // Lấy sản phẩm theo từ khóa
            var searchResults = await productService.GetProductByName(searchKey);

            // Trả về Index view với kết quả tìm kiếm
            return View("Index", searchResults.ToList());

        }
    }
}
