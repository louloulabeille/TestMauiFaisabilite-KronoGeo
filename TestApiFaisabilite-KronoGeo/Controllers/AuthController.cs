using Microsoft.AspNetCore.Mvc;

namespace TestApiFaisabilite_KronoGeo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        [HttpPost]
        [Route("login")]
        public async  Task<IActionResult> Login()
        {
            return View();
        }
    }
}
