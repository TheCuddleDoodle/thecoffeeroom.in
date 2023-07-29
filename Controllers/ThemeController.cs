using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Coffeeroom.Controllers
{
    [Route("api/themer")]
    public class ThemeController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            List<Theme> themes = new();
            string connectionString = ConfigHelper.NewConnectionString;
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                string sql;
                sql = "SELECT * FROM TblThemeMaster WHERE Id=@Id";
                using SqlCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                using SqlDataReader dataReader = await command.ExecuteReaderAsync();

                while (await dataReader.ReadAsync())
                {
                    Theme entry = new()
                    {
                        Title = (string)dataReader["Title"],
                        Description = (string)dataReader["Description"],
                        String = (string)dataReader["String"],
                        FontLink = (string)dataReader["FontLink"],
                    };
                    themes.Add(entry);
                }
            }
            return new JsonResult(themes);
        }

    }
}
