using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Coffeeroom.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Coffeeroom.Controllers
{
    
    public class NewsLetterController : Controller
    {

        public readonly IMailingListRepo _mailingListRepo;
        public readonly IUserProfileRepo _userProfileRepo;
        public string connectionString = ConfigHelper.NewConnectionString;

        public NewsLetterController(IMailingListRepo mailingListRepo,IUserProfileRepo userProfileRepo)
        {
            _mailingListRepo = mailingListRepo;
            _userProfileRepo = userProfileRepo;
        }

        [HttpPost]
        [Route("api/newsletter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMailToLetter([FromBody] Mail mail)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    try
                    {
                        await _mailingListRepo.AddAsync(mail);
                        return Ok("email submitted!!");
                        
                    }
                    catch(Exception ex)
                    {
                        Log.Error("email submission in page:" + ex.Message.ToString());
                        return BadRequest(ex.Message.ToString());
                    }
                }
                else
                {
                    return BadRequest("Invalid form data");
                }
               
            }
            catch (Exception ex)
            {
                Log.Error("error while email submission:" + ex.Message.ToString());
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}
