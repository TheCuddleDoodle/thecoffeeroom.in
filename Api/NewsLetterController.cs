using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Coffeeroom.Api
{
    public class NewsLetterController : Controller
    {
        public string connectionString = ConfigHelper.NewConnectionString;
       
        [HttpPost]
        [Route("api/newsletter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMailToLetter([FromBody] Mail mail)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    using SqlConnection connection = new(connectionString);
                    await connection.OpenAsync();

                    string checkEmailQuery = "SELECT COUNT(*) FROM TblMailingList WHERE Email = @Email";
                    SqlCommand checkEmailCmd = new(checkEmailQuery, connection);
                    checkEmailCmd.Parameters.AddWithValue("@Email", mail.EMailId);
                    int emailCount = (int)await checkEmailCmd.ExecuteScalarAsync();

                    if (emailCount == 0)
                    {
                        string getMaxIdQuery = "SELECT MAX(Id) FROM TblMailingList";
                        SqlCommand getMaxIdCmd = new(getMaxIdQuery, connection);
                        int maxId = (int)await getMaxIdCmd.ExecuteScalarAsync();
                        int newId = maxId + 1;

                        string addEmailQuery = "INSERT INTO TblMailingList (Id, Email) VALUES (@Id, @Email)";
                        SqlCommand addEmailCmd = new(addEmailQuery, connection);
                        addEmailCmd.Parameters.AddWithValue("@Id", newId);
                        addEmailCmd.Parameters.AddWithValue("@Email", mail.EMailId);
                        await addEmailCmd.ExecuteNonQueryAsync();
                        Log.Information("mail added to newsletter:" + mail.EMailId);
                        return Ok("Mail Added Successfully");
                    }
                    else
                    {
                        return BadRequest("Email already present");
                    }
                }
                else
                {
                    return BadRequest("invalid form data");
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
