using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
