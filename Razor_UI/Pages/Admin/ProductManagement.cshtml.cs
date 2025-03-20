using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Razor_UI.SignalRHub;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Razor_UI.Pages.Admin
{
    public class ProductManagementModel : PageModel
    {
        private readonly ICrudRepo<Product, int> _productRepo;
        private readonly IHubContext<DataSignalR> _hubContext;
        private readonly ICrudRepo<Category, int> _categoryRepo;


        [BindProperty]
        public List<Product> Products { get; set; } = new List<Product>();

 
        public ProductManagementModel(ICrudRepo<Product, int> productRepo, IHubContext<DataSignalR> hubContext, ICrudRepo<Category, int> categoryRepo)
        {
            _productRepo = productRepo;
            _hubContext = hubContext;
            _categoryRepo = categoryRepo;
        }

        public async Task OnGetAsync()
        {
            Products = (await _productRepo.GetAllWithInclude(p => p.Category, p => p.Carts, p => p.OrderDetails)).ToList();
            ViewData["categoryProduct"] = new SelectList(await _categoryRepo.GetAll() , "CategoryId", "CategoryName");

        }


        [BindProperty]
        public Product Product { get; set; } = new Product();



        public async Task<IActionResult> OnPostCreateAsync(IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
                return RedirectToPage("/Admin/ProductManagement");

            if (ImageFile != null)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePathMVC = Path.Combine("../PRN222_Final_Project/wwwroot/img/PRODUCT/", fileName);
                var filePathRazor = Path.Combine("wwwroot/img/PRODUCT/", fileName);

                using (var stream = new FileStream(filePathMVC, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                using (var stream = new FileStream(filePathRazor, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                Product.ImageUrl = "img/PRODUCT/" + fileName;
            }

            await _productRepo.Add(Product);
            await _hubContext.Clients.All.SendAsync("loadProduct");

            return RedirectToPage("/Admin/ProductManagement");
        }


        public async Task<IActionResult> OnPostEditAsync(IFormFile ImageFile)
        {
            if (Product.ProductId == 0 && Product.ProductName == null && Product.Price == 0 
                && Product.Description == null && Product.StockQuantity == 0 && Product.CategoryId == 0)
                return RedirectToPage("/Admin/ProductManagement");

            if (ImageFile != null)
            {
                if (ImageFile.ToString() != Product.ImageUrl)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePathMVC = Path.Combine("../PRN222_Final_Project/wwwroot/img/PRODUCT/", fileName);
                    var filePathRazor = Path.Combine("wwwroot/img/PRODUCT/", fileName);

                    using (var stream = new FileStream(filePathMVC, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(filePathRazor, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    Product.ImageUrl = "img/PRODUCT/" + fileName;
                }
            }
            else
            {
                Product.ImageUrl = Product.ImageUrl;
            }

            await _productRepo.Update(Product);
            await _hubContext.Clients.All.SendAsync("loadProduct");

            return RedirectToPage("/Admin/ProductManagement");
        }



        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _productRepo.GetById(id);

            if (product == null)
            {
                return RedirectToPage("/Admin/ProductManagement"); // Trả về lỗi 404 nếu không tìm thấy sản phẩm
            }

            await _productRepo.Delete(product);
            await _hubContext.Clients.All.SendAsync("loadProduct");

            return RedirectToPage("/Admin/ProductManagement");
        }



        

        [HttpPost]
        public async Task<IActionResult> OnPostSearchAsync(IFormCollection form)
        {
            try
            {

                string searchTerm = form["SearchTerm"];
                string categorySearch = form["categorySearch"];
                var productQuery = await _productRepo.GetAllWithInclude(c => c.Category);

                var products = productQuery
                    .Where(p =>
                        (string.IsNullOrEmpty(searchTerm) ||
                         p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                         p.Description.ToLower().Contains(searchTerm
                         .ToLower())) &&
                        (string.IsNullOrEmpty(categorySearch) ||
                         (p.Category != null && p.Category.CategoryId.ToString() == categorySearch))
                    )
                    .Select(p => new Product
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        CategoryId = p.CategoryId, // Sử dụng CategoryId thay vì CategoryName
                        ImageUrl = p.ImageUrl,
                        IsAvailable = p.IsAvailable,
                        CreatedDate = p.CreatedDate,
                        Category = p.Category // Bao gồm cả object Category nếu cần
                    }).ToList();

                ViewData["categoryProduct"] = new SelectList(await _categoryRepo.GetAll(), "CategoryId", "CategoryName");

                Products = products;

                // Property giữ nguyên


                return Page();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }
    }
}
