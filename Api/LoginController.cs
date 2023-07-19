using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coffeeroom.Api
{
    [Route("/api/autologin")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        //get request and search for hash stored if not set default params as browser state
    }
}
