using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coffeeroom.Api
{
    [Route("api/autologin")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //get request and search for hash stored if not set default params as browser state
    }
}
