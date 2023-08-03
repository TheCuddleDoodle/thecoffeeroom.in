using Coffeeroom.Core.Helpers;
using Coffeeroom.Models;
using Coffeeroom.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text.Json;

namespace Coffeeroom.Pages.Account
{
    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {
        public void OnGet()
        {

        }

        public async Task<JsonResult> OnPostLoginSubmitAsync([FromBody] JsonElement jsonData)
        {
            string connectionString = ConfigHelper.NewConnectionString;
            JsonDocument jsonDocument = JsonDocument.Parse(jsonData.GetRawText());

            string message = "something went wrong", type = "error";
            string UserName = jsonDocument.RootElement.GetProperty("userName").GetString()?.Trim().ToLower() ?? string.Empty;
            string Password = jsonDocument.RootElement.GetProperty("passWord").GetString()?.Trim() ?? string.Empty;

            jsonDocument.Dispose();

            if (UserName == string.Empty)
            {
                message = "Enter a valid username";
            }
            else if (Password == string.Empty)
            {
                message = "Enter a valid password";
            }
            else
            {
                try
                {
                    using SqlConnection connection = new(connectionString);
                    await connection.OpenAsync();
                    SqlCommand checkcommand = new("select p.*,a.Image " +
                        "from TblUserProfile p,TblAvatarMaster a " +
                        "where UserName = @username " +
                        "and PassWord =@password " +
                        "and p.IsActive= 1 " +
                        "and p.IsVerified = 1 " +
                        "and p.AvatarId = a.Id", connection);
                    checkcommand.Parameters.AddWithValue("@username", UserName.ToLower());
                    checkcommand.Parameters.AddWithValue("@password", Password);
                    using (var reader = await checkcommand.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var username = reader.GetString(reader.GetOrdinal("UserName"));
                            var user_id = reader.GetInt32(reader.GetOrdinal("Id"));
                            var firstname = reader.GetString(reader.GetOrdinal("FirstName"));
                            var fullname = firstname + " " + reader.GetString(reader.GetOrdinal("LastName"));
                            var role = reader.GetString(reader.GetOrdinal("Role"));
                            var avatar = reader.GetString(reader.GetOrdinal("Image"));
                            //set session
                            HttpContext.Session.SetString("user_id", user_id.ToString());
                            HttpContext.Session.SetString("username", username);
                            HttpContext.Session.SetString("first_name", firstname);
                            HttpContext.Session.SetString("role", role);
                            HttpContext.Session.SetString("fullname", fullname);
                            HttpContext.Session.SetString("avatar", avatar.ToString());
                            message = "logging in...";
                            type = "success";
                        }
                        else
                        {
                            message = "Invalid Credentials";
                            Log.Information("invalid creds by username:" + UserName);
                        }
                    }
                    await connection.CloseAsync();


                }
                catch (Exception ex)
                {
                    message = "something went wrong " + ex.Message.ToString();
                   Log.Error(ex.Message.ToString() + " : in login form,user : " + UserName);
                }
            }

            var keys = new
            {
                message,
                type
            };
            return new JsonResult(keys);


        }
    }
}
