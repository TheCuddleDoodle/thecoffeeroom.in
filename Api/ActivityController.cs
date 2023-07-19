using Microsoft.AspNetCore.Mvc;

namespace Coffeeroom.Api
{
    [Route("api/autologin")]
    [ApiController]
    public class ActivityController
    {
        public bool AddActivity(JsonResult activity)
        {
            try{
                //enter activity
            }
            catch{
                //error
            }
            return true;
        }
    }
}
