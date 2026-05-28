using Microsoft.AspNetCore.Mvc;

namespace TestApiFaisabilite_KronoGeo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        [HttpPost]
        public IActionResult Index()
        {
            return View();
        }
    }
}
