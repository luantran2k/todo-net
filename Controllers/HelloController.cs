using Microsoft.AspNetCore.Mvc;

namespace MyApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello from .NET API heileo!");
        }

    }
}
