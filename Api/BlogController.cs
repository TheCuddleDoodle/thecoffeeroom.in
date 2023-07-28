using Microsoft.AspNetCore.Mvc;

namespace Coffeeroom.Api
{
    [ApiController]
    public class BlogController : Controller
    {
        [HttpGet("api/blogs/firstthree")]
        public async Task<IActionResult> GetFirstThree()
        {
            return Ok("done successfully");
        }
    }
}
