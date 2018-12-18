using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult GetSomething()
        {
            return Ok();
        }
    }
}
