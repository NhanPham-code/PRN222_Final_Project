using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{
    public class ContactController : Controller
    {
        private ICrudRepo<Category, int> _categoryRepo;

        public ContactController(ICrudRepo<Category, int> _categoryRepo)
        {
            this._categoryRepo = _categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            // get category
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }


            return View();
        }
    }
}
