using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Coffeeroom.Pages.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Coffeeroom.Pages.Blogs
{
    public class BlogThumbz
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string UrlHandle { get; set; }
        public string PostContent { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Tags { get; set; }
        public string Yr { get; set; }
        public string Locator { get; set; }
        public string PostLikes { get; set; }
        public int Comments { get; set; }
        public DateTime DatePosted { get; set; }
    }
    public class BlogTriggers
    {
        public string Mode { get; set; }
        public string Classifypost { get; set; }
        public string Keypost { get; set; }

    }
    public class BlogCat
    {
        public string Category { get; set; }
        public string BlogNum { get; set; }

    }
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        public List<BlogCategory> Cats { get; set; }
        public List<BlogCategory> Years { get; set; }
        public List<BlogCategory> Tags { get; set; }
        public string Classify { get; set; }
        public string Key { get; set; }
        public async Task OnGetAsync(string classify, string key)
        {
            if (classify != null && key != null)
            {
                Classify = classify;
                Key = key;
            }
            else
            {
                Classify = "na";
                Key = "na";
            }
            await LoadCategories();


        }
        public async Task<JsonResult> OnPostFetchBlogsAsync([FromBody] BlogTriggers blogTriggers)
        {
            List<BlogThumbz> thumbs = new();
            _ = new List<BlogThumbz>();
            if (blogTriggers.Mode != "n")
            {
                string connectionString = ConfigHelper.NewConnectionString;
                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();
                string sql = "";
                if (blogTriggers.Classifypost != "na" && blogTriggers.Keypost != "na")
                {
                    if (blogTriggers.Classifypost == "category")
                    {
                        sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                              "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                              "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                              "WHERE c.Locator = '" + blogTriggers.Keypost + "'" +
                              "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                              "ORDER BY Id OFFSET " + blogTriggers.Mode + " " +
                              "ROWS FETCH NEXT 2 ROWS ONLY";
                    }
                    else if (blogTriggers.Classifypost == "year")
                    {
                        sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                             "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                             "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                             "WHERE m.CategoryId = c.Id and YEAR(m.DatePosted) = '" + blogTriggers.Keypost + "' AND m.IsActive = 1" +
                             "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                             "ORDER BY Id OFFSET " + blogTriggers.Mode + " " +
                             "ROWS FETCH NEXT 2 ROWS ONLY";

                    }
                    else if (blogTriggers.Classifypost == "tag")
                    {
                        sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                             "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                             "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                             "WHERE m.CategoryId = c.Id and Tags like '%" + blogTriggers.Keypost + "%' AND m.IsActive = 1" +
                             "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                             "ORDER BY Id OFFSET " + blogTriggers.Mode + " " +
                             "ROWS FETCH NEXT 2 ROWS ONLY";
                    }


                }
                else
                {
                    sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                            "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                            "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                            "WHERE m.CategoryId = c.Id  AND m.IsActive = 1" +
                            "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                            "ORDER BY Id OFFSET " + blogTriggers.Mode + " " +
                            "ROWS FETCH NEXT 2 ROWS ONLY";
                }
                using SqlCommand command = new(sql, connection);
                using SqlDataReader dataReader = await command.ExecuteReaderAsync();

                if (dataReader.HasRows)
                {
                    while (await dataReader.ReadAsync())
                    {
                        thumbs.Add(new BlogThumbz
                        {
                            Title = dataReader.GetString(1),
                            Description = dataReader.GetString(2),
                            UrlHandle = dataReader.GetString(3),
                            DatePosted = dataReader.GetDateTime(4),
                            Category = dataReader.GetString(7),
                            Locator = dataReader.GetString(8),
                            Comments = dataReader.GetInt32(9)
                        });
                    }
                }
                await connection.CloseAsync();
            }
            //Thread.Sleep(2000);
            return new JsonResult(thumbs);
        }

        public async Task LoadCategories()
        {

            Cats = new List<BlogCategory>();
            string connectionString = ConfigHelper.NewConnectionString;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(
              "SELECT bc.Title,COUNT(bp.CategoryId) AS Occurrences FROM TblBlogCategory bc " +
              "LEFT JOIN TblBlogMaster bp ON bc.Id = bp.CategoryId WHERE bc.IsActive = 1 " +
              "GROUP BY bc.Title;", connection
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
        public async Task<JsonResult> OnGetLatestPostsAsync([FromBody] Blog blog)
        {
            try
            {
                List<Blog> entries = new();
                string connectionString = ConfigHelper.NewConnectionString;
                using (SqlConnection connection = new(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "Select * from TblBlogMaster where IsActive = 1";
                    using SqlCommand command = new(sql, connection);
                    using SqlDataReader dataReader = await command.ExecuteReaderAsync();

                    while (dataReader.Read())
                    {
                        Blog entry = new()
                        {
                            Title = (string)dataReader["Title"],
                            Description = (string)dataReader["Description"],
                            UrlHandle = (string)dataReader["UrlHandle"]
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
                return new JsonResult(errorResponse);
            }

        }



    }
}
