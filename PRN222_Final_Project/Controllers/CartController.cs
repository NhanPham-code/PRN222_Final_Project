using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View(); 
        }
    }
}
