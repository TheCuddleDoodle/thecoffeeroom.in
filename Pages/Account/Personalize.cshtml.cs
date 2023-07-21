using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Coffeeroom.Pages.Account
{
    public class PersonalizeModel : PageModel
    {
        readonly string connectionString = ConfigHelper.NewConnectionString;
        public List<Theme> ThemeDD { get; set; }
        public async Task OnGetAsync()
        {
            await LoadThemes();
        }

        public async Task LoadThemes()
        {
            ThemeDD = new List<Theme>();
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT a.*,u.FirstName as CuratorName from TblThemeMaster a,TblUserProfile u where a.CuratorId = u.Id order by Id", connection);
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ThemeDD.Add(new Theme
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    String = reader.GetString(reader.GetOrdinal("String")),
                    FontLink = reader.GetString(reader.GetOrdinal("FontLink")),
                    CuratorId = reader.GetInt32(reader.GetOrdinal("CuratorId")),
                    CuratorName = reader.GetString(reader.GetOrdinal("CuratorName")),
                    CoverImage = reader.GetString(reader.GetOrdinal("CoverImage")),
                    PrimaryCol = reader.GetString(reader.GetOrdinal("PrimaryCol")),
                });
            }
            await reader.CloseAsync();

        }
    }
}
