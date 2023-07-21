using Coffeeroom.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Coffeeroom.Pages.Account
{
    public class PassCode
    {
        public string NewPass { get; set; }
        public string ConfirmPass { get; set; }
    }

    [ValidateAntiForgeryToken]
    public class SecurityModel : PageModel
    {
        string connectionString = ConfigHelper.NewConnectionString;
        public IActionResult OnGet()
        {
            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                return Partial("Pages/Shared/ErrorPages/_AccessDenied.cshtml");
              
            }
            else
            {
                return Page();
            }
        }

        public async Task<JsonResult> OnPostSubmitPass([FromBody] PassCode passCode)
        {
            string message = "something went wrong", type = "error";
            if (passCode.NewPass != null && passCode.ConfirmPass != null)
            {
                if (passCode.NewPass == "")
                {
                    message = "enter the password";
                    type = "error";
                }
                else if (passCode.NewPass.Length <= 5)
                {
                    message = "Password needs to be of atleast 6 digits";
                    type = "error";
                }
                else if (passCode.ConfirmPass == "")
                {
                    message = "Confirm your password";
                    type = "error";
                }
                else if (passCode.NewPass != passCode.ConfirmPass)
                {
                    message = "Passwords don't match";
                    type = "error";
                }
                else
                {
                    try
                    {
                        using SqlConnection connection = new(connectionString);

                        await connection.OpenAsync();
                        SqlCommand insertCommand = new("UPDATE TblUserProfile SET Password = @Password where UserName = @UserName", connection);
                        insertCommand.Parameters.AddWithValue("@Password", passCode.NewPass.Trim());
                        insertCommand.Parameters.AddWithValue("@UserName", HttpContext.Session.GetString("username"));
                        await insertCommand.ExecuteNonQueryAsync();
                        message = "Changes Saved!!";
                        type = "success";
                        await connection.CloseAsync();
                    }
                    catch
                    {
                        message = "something went wrong exc";
                        type = "error";
                    }
                }
            }
            else
            {
                message = "something went wrong:";
                type = "error";
            }
            var keys = new { message, type };
            return new JsonResult(keys);
        }
    }
}
