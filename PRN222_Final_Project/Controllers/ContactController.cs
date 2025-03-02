using Microsoft.AspNetCore.Mvc;

namespace PRN222_Final_Project.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
