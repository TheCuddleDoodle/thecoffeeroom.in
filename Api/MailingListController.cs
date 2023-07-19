using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace Coffeeroom.Api
{
    [Route("api/addtomailinglist")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class MailingListController : ControllerBase
    {
        private readonly ILogger<MailingListController> _logger;

        public MailingListController(ILogger<MailingListController> logger)
        {
            _logger = logger;
        }
        public async Task<JsonResult> OnPostSubscribeToMailingList([FromBody] Mail mail)
        {
            try
            {
                
                string sql;
                string connectionString = ConfigHelper.NewConnectionString;
                using (SqlConnection connection = new(connectionString))
                {
                    await connection.OpenAsync();
                    sql = "INSERT INTO TblMailingList(Email) Values('jskainthofficial@gmail.com')";

                    using SqlCommand command = new(sql, connection);
                    //command.Parameters.AddWithValue("@Type", );
                    using SqlDataReader dataReader = await command.ExecuteReaderAsync();

                }
                EventModel evnt = new EventModel
                {
                    Message = "",
                    Type = ""
                };
                return new JsonResult(evnt);
            }
            catch (Exception ex)
            {
                EventModel evnt = new EventModel
                {
                    Message = "",
                    Type = ""
                };
                _logger.LogInformation("error from catch block of live search : " + ex.Message.ToString());
                return new JsonResult(evnt);
            }
        }
    }
}
