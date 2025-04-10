using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Razor_UI.SignalRHub;

namespace Razor_UI.Pages.Admin
{
    public class CategoryManagementModel : PageModel
    {
        private readonly ICrudRepo<Category, int> _categoryRepo;
        private readonly ICrudRepo<Product, int> _productRepo;
        private readonly IHubContext<DataSignalR> _hubContext;

        [BindProperty]
        public List<Category> Categories { get; set; } = new List<Category>();

        [BindProperty]
        public Category Category { get; set; } = new Category();

        [BindProperty]
        public List<Product> Products { get; set; } = new List<Product>();

        public CategoryManagementModel(ICrudRepo<Category, int> categoryRepo, ICrudRepo<Product, int> productRepo, IHubContext<DataSignalR> hubContext)
        {
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
            _hubContext = hubContext;
        }

        public async Task OnGetAsync(string? search, string? category)
        {
            var allCategories = await _categoryRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                allCategories = allCategories.Where(c => c.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                allCategories = allCategories.Where(c => c.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            Categories = allCategories.ToList();
        }

        // Create Category
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _categoryRepo.Add(Category);
            await _hubContext.Clients.All.SendAsync("Dang");

            return RedirectToPage();
        }

        // Update Category
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _categoryRepo.Update(Category);
            await _hubContext.Clients.All.SendAsync("loadCateGory");

            return RedirectToPage();
        }

        // Delete Category
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var category = await _categoryRepo.GetById(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category does not exist!";
                return RedirectToPage();
            }

            // Kiểm tra xem có sản phẩm nào liên kết với category không
            var productsInCategory = await _productRepo.GetAll();
            bool hasLinkedProducts = productsInCategory.Any(p => p.CategoryId == id);

            if (hasLinkedProducts)
            {
                TempData["WarningMessage"] = "This category has linked products! Deletion is not allowed.";
                TempData["CanDelete"] = false; 
            }
            else
            {
                TempData["WarningMessage"] = "Are you sure you want to delete this category?";
                TempData["CategoryIdToDelete"] = id;
                TempData["CanDelete"] = true; 
            }

            return RedirectToPage();
        }


        // Confirmation of deletion
        public async Task<IActionResult> OnPostDeleteConfirmedAsync(int id)
        {
            var category = await _categoryRepo.GetById(id);
            if (category != null)
            {
                await _categoryRepo.Delete(category);
                await _hubContext.Clients.All.SendAsync("loadCateGory");
            }
            return RedirectToPage();
        }





    }


}
