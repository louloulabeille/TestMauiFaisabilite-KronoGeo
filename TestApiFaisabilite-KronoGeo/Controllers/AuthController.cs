using Microsoft.AspNetCore.Mvc;

namespace TestApiFaisabilite_KronoGeo.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
