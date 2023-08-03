using Coffeeroom.Core.Helpers;
using Coffeeroom.Pages.Blogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Coffeeroom.Controllers
{
    [ApiController]
    public class BlogController : Controller
    {
        [HttpGet("api/blogs/{mode}/{classify}/{key}")]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> OnPostFetchBlogsAsync(string mode,string classify,string key)
        {
            mode = "0";
            List<BlogThumbz> thumbs = new();
            _ = new List<BlogThumbz>();
            if ( mode != "n")
            {
                string connectionString = ConfigHelper.NewConnectionString;
                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();
                string sql = "";
                if (classify != "na" && key != "na")
                {
                    if (classify == "category")
                    {
                        sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                              "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                              "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                              "WHERE c.Locator = '" + key + "' and m.IsActive = 1" +
                              "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                              "ORDER BY Id OFFSET " + mode + " " +
                              "ROWS FETCH NEXT 2 ROWS ONLY";
                    }
                    else if (classify == "year")
                    {
                        sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                             "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                             "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                             "WHERE m.CategoryId = c.Id and YEAR(m.DatePosted) = '" + key + "' AND m.IsActive = 1" +
                             "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                             "ORDER BY Id OFFSET " + mode + " " +
                             "ROWS FETCH NEXT 2 ROWS ONLY";

                    }
                    else if ( classify == "tag")
                    {
                        sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                             "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                             "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                             "WHERE m.CategoryId = c.Id and Tags like '%" + key + "%' AND m.IsActive = 1" +
                             "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                             "ORDER BY Id OFFSET " + mode + " " +
                             "ROWS FETCH NEXT 2 ROWS ONLY";
                    }

                    else if (classify == "search")
                    {
                        sql = "SELECT m.Id, m.Title,m.Description,m.UrlHandle,m.DatePosted,m.Tags,YEAR(m.DatePosted) AS Year,c.Title AS Category,c.Locator,COUNT(bc.Id) AS Comments FROM TblBlogMaster m " +
                             "LEFT JOIN TblBlogComment bc ON m.Id = bc.PostId " +
                             "JOIN TblBlogCategory c ON m.CategoryId = c.Id " +
                             "WHERE m.CategoryId = c.Id and m.Title like '%" + key + "%' AND m.IsActive = 1" +
                             "GROUP BY m.Id, m.Title,m.Description,m.UrlHandle, m.DatePosted,m.Tags,c.Title,c.Locator " +
                             "ORDER BY Id OFFSET " + mode + " " +
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
                            "ORDER BY Id OFFSET " + mode + " " +
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
            return new JsonResult(thumbs);
        }
    }
}
