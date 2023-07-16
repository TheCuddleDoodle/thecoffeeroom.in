using Coffeeroom.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Coffeeroom.Api.Blog
{
    [ApiController]
    public class BlogController : ControllerBase
    {
        //[HttpGet]
        //[Route("api/blog/list")]
        //public async Task<JsonResult> OnGetGetYearListAsync()
        //{
        //    try
        //    {
        //        List<FinYear> entries = new();
        //        string connectionString = ConfigHelper.NewConnectionString;
        //        using (SqlConnection connection = new(connectionString))
        //        {
        //            await connection.OpenAsync();
        //            string sql = "select * from M0501_FinancialYear";
        //            using SqlCommand command = new(sql, connection);
        //            using SqlDataReader dataReader = await command.ExecuteReaderAsync();

        //            while (dataReader.Read())
        //            {
        //                FinYear entry = new()
        //                {
        //                    Code = (string)dataReader["Code"],
        //                    StartYear = dataReader["StartYear"].ToString(),
        //                    EndYear = DateTime.Parse((dataReader["EndYear"].ToString())).ToString()

        //                    //StartYear = DateTime.ParseExact(dataReader["StartYear"].ToString(), "yyyy-MM-dd HH:mm:ss fff", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss.fff")
        //                    //EndYear = (DateTime)dataReader["EndYear"],
        //                };
        //                entries.Add(entry);
        //            }
        //            await connection.CloseAsync();
        //        }
        //        return new JsonResult(entries);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new
        //        {
        //            messageType = "error",
        //            messageValue = ex.Message
        //        };
        //        //  _logger.LogInformation("error from catch block of OnGetBranchList: " + ex.Message.ToString());
        //        return new JsonResult(errorResponse);
        //    }
        //}
    }
}
