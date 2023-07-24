using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Data.SqlClient;

namespace Coffeeroom.Api
{
    public class NewsLetterController : Controller
    {
        [HttpPost]
        [Route("api/newsletter")]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> AddMailToLetter([FromBody] Mail mail)
        {
            string sql, sqlcheck;
            string connectionString = ConfigHelper.NewConnectionString;
            string message, type;
            try
            {
                if (mail.EMailId.Trim().Length <= 4 || !Validators.IsValidEmail(mail.EMailId.Trim()))
                {
                    message = "Invalid Email";
                    type = "error";
                }
                else
                {
                    using (SqlConnection connection = new(connectionString))
                    {
                        await connection.OpenAsync();

                        sqlcheck = "select max(Id)+1 from TblMailingList";
                        using SqlCommand checkcommand = new(sqlcheck, connection);
                        object result = await checkcommand.ExecuteScalarAsync();
                        string maxval = result != null ? result.ToString() : "1";

                        sql = "INSERT INTO TblMailingList(Id,Email,DateAdded) Values(@Id,@mail,@today)";
                        using SqlCommand command = new(sql, connection);
                        command.Parameters.AddWithValue("@Id", maxval);
                        command.Parameters.AddWithValue("@mail", mail.EMailId.Trim().ToLower());
                        command.Parameters.AddWithValue("@today", DateTime.Now.ToString());
                        using SqlDataReader dataReader = await command.ExecuteReaderAsync();
                    }
                    message = "mail submitted";
                    type = "success";
                }
            }
            catch (Exception ex)
            {

                message = "Something went wrong" + ex.Message.ToString();
                    type = "error";
               
                Log.Information("error from catch block of mailing list controller : " + ex.Message.ToString());
            }

            EventModel evnt = new EventModel
            {
                Message = message,
                Type = type
            };
            return new JsonResult(evnt);
        }
    }
}
