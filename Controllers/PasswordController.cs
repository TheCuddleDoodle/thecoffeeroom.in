using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Coffeeroom.Controllers
{
    [ApiController]
    public class PasswordController : ControllerBase
    {

        public readonly IPassResetTokenRepo _passresetrepo;
        public PasswordController(IPassResetTokenRepo passResetTokenRepo)
        {
            _passresetrepo = passResetTokenRepo;
        }

          


        [HttpPost("api/passreset")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SubmitPass(PasswordToken passwordToken)
        {

           // string checkUserQuery = "SELECT COUNT(*) FROM TblUserProfile WHERE UserId = @userid or EMailId = @userid";
           // SqlCommand checkUserCmd = new(checkUserQuery, connection);
           // checkUserCmd.Parameters.AddWithValue("@userid", passwordToken.UserId);
           // int userCount = (int)await checkUserCmd.ExecuteScalarAsync();


            if (ModelState.IsValid)
            {

                string secret = StringProcessors.GenerateRandomString(10);
                var passwordPass = new PasswordToken
                {

                    //UserId = ,
                    DateAdded = DateTime.Now,
                    Token = secret,
                    IsValid = true

                };
                try
                {
                  await  _passresetrepo.AddToken(passwordPass);
                    return Ok("reset mail send,check your mail");
                   
                }
                catch(Exception ex)
                {
                    return BadRequest("something:" + ex.Message.ToString());
                }
            }
            else
            {
                return BadRequest("invalid form data");
            }
        }
    }
}
