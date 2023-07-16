using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Coffeeroom.Api
{
    public class LiveSearchController : ControllerBase
    {
        [HttpGet]
        [Route("/api/livesearch/{Type?}/{SearchKey?}")]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> OnGetEngineOilList(string Type,string SearchKey)
        {
            try
            {
                List<LiveSearch> entries = new();
                string connectionString = ConfigHelper.NewConnectionString;
                using (SqlConnection connection = new(connectionString))
                {
                    connection.Open();
                    string sql = "select * from TblSearchMaster";

                    using SqlCommand command = new(sql, connection);
                    command.Parameters.AddWithValue("@SString", "%" + SearchKey + "%");
                    command.Parameters.AddWithValue("@Type", Type);
                    using SqlDataReader dataReader = await command.ExecuteReaderAsync();

                    while (await dataReader.ReadAsync())
                    {
                        LiveSearch entry = new()
                        {
                            Title = (string)dataReader["Title"],
                            Description = (string)dataReader["Description"],
                            Image = (string)dataReader["Image"],
                            Url = (string)dataReader["Url"],
                            Type = (string)dataReader["Type"]
                        };
                        entries.Add(entry);
                    }
                }
                return new JsonResult(entries);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    messageType = "error",
                    messageValue = ex.Message
                };
                //  _logger.LogInformation("error from catch block of OnGetBranchList: " + ex.Message.ToString());
                return new JsonResult(errorResponse);
            }
        }
    }
}
