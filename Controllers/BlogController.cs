﻿using Microsoft.AspNetCore.Mvc;

namespace Coffeeroom.Controllers
{
    [ApiController]
    public class BlogController : Controller
    {
        [HttpGet("api/blogs/firstthree")]
        public  IActionResult GetFirstThree()
        {
            return Ok("done successfully");
        }
    }
}