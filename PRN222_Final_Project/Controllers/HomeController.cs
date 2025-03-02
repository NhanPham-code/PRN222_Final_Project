using BLL.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PRN222_Final_Project.Controllers
{
    public class HomeController : Controller
    {
        private ICrudRepo<Product, int> _productRepo;
        private ICrudRepo<Category, int> _categoryRepo;

        public HomeController(ICrudRepo<Product, int> _productRepo, ICrudRepo<Category, int> categoryRepo)
        {
            this._productRepo = _productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            // get category
            var categories = (await _categoryRepo.GetAll()).ToList();
            if (categories != null) 
            {
                ViewBag.Categories = categories;
            }

            // get bestseller
            var rainbow = await _productRepo.GetById(22);
            var bunny = await _productRepo.GetById(8);
            var lava = await _productRepo.GetById(2);
            if(rainbow != null && bunny != null && lava != null)
            {
                ViewData["rainbow"] = rainbow;
                ViewData["bunny"] = bunny;
                ViewData["lava"] = lava;
            }
            else
            {
                ViewData["rainbow"] = null;
                ViewData["bunny"] = null;
                ViewData["lava"] = null;
            }
           
            return View();
        }
    }
}
