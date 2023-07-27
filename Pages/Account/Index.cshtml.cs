using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Coffeeroom.Pages.Account
{
    public class IndexModel : PageModel
    {
        string connectionString = ConfigHelper.NewConnectionString;
        public UserProfile UserDetailsDisplay { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var sessionStat = HttpContext.Session.GetString("role");
            if (sessionStat != null)
            {
                if (sessionStat.ToString() == "user" || sessionStat.ToString() == "admin")
                {
                    using var connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();
                    await LoadUserData(connection);
                    await connection.CloseAsync();
                    return Page();
                }
                else
                {
                    return LocalRedirect("/account/login");
                }
                
            }
            else
            {
                return LocalRedirect("/account/login");
            }
        }

        public async Task LoadUserData(SqlConnection connection)
        {
            var command = new SqlCommand("SELECT * FROM TblUserProfile WHERE UserName = @Id", connection);
            command.Parameters.AddWithValue("@Id", HttpContext.Session.GetString("username"));
            var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                UserDetailsDisplay = new UserProfile()
                {
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    UserName = reader.GetString(3),
                    Role = reader.GetString(11),
                    EMail = reader.GetString(4),
                    Phone = reader.GetString(5),
                    Gender = reader.GetString(6),
                    Bio = reader.GetString(12),
                    AvatarId = reader.GetInt32(13)
                };
                await reader.CloseAsync();
            }
            else
            {
                //return NotFound();
            }
        }
    }
}
