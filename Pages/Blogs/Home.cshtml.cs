using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Coffeeroom.Pages.Blogs
{
    public class HomeModel : PageModel
    {
        public List<BlogCategory> Cats { get; set; }
        public async Task OnGetAsync()
        {
            await LoadCategories();
        }

        public async Task LoadCategories()
        {

            Cats = new List<BlogCategory>();
            string connectionString = ConfigHelper.NewConnectionString;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(
              "SELECT bc.Title,COUNT(bp.CategoryId) AS Occurrences FROM TblBlogCategory bc " +
              "LEFT JOIN TblBlogMaster bp ON bc.Id = bp.CategoryId AND bp.IsActive = 1 " +
              "WHERE bc.IsActive = 1 GROUP BY bc.Title ", connection
            );
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Cats.Add(new BlogCategory
                {
                    Title = reader.GetString(0),
                    Locator = reader.GetString(0).ToLower(),
                    Counter = reader.GetInt32(1)
                });
            }
            await reader.CloseAsync();


        }

    }
}
