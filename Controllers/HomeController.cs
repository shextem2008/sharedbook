
using Microsoft.AspNetCore.Mvc;

namespace SHB.WebApi.Controllers
{

    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("SharedBook Api is running");
        }
    }
}
