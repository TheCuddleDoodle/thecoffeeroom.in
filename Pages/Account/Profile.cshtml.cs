using Coffeeroom.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using Coffeeroom.Models.Domain;

namespace Coffeeroom.Pages.Account
{
   

    [ValidateAntiForgeryToken]
    public class ProfileModel : PageModel
    {
        string connectionString = ConfigHelper.NewConnectionString;
        public UserProfile UserDetailsDisplay { get; set; }

        public List<Avatar> AvatarDD { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {

            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                return Partial("Pages/Shared/ErrorPages/_AccessDenied.cshtml");
            }
            else
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                await LoadUserData(connection);
                await LoadAvatarDropdown(connection);
                await connection.CloseAsync();
                string fname = "", lname = "", role = "";
                if (UserDetailsDisplay != null)
                {
                    if (UserDetailsDisplay.FirstName != null)
                    {
                        fname = UserDetailsDisplay.FirstName.ToString();
                    }
                    else
                    {
                        fname = "";
                    }
                    if (UserDetailsDisplay.LastName != null)
                    {
                        lname = UserDetailsDisplay.LastName.ToString();
                    }
                    else
                    {
                        lname = "";
                    }
                    if (UserDetailsDisplay.Role != null)
                    {
                        role = UserDetailsDisplay.Role.ToString();
                    }
                }

                ViewData["role"] = role;
                ViewData["fullname"] = fname + " " + lname;
                return Page();
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

        public async Task LoadAvatarDropdown(SqlConnection connection)
        {
            AvatarDD = new List<Avatar>();
            var command = new SqlCommand("SELECT * FROM TblAvatarMaster ", connection);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                AvatarDD.Add(new Avatar
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Image = reader.GetString(2),
                    Description = reader.GetString(3)
                });
            }
            await reader.CloseAsync();

        }

        public async Task<JsonResult> OnPostSubmitDeets([FromBody] UserProfile EditProfile)
        {

            string message = "something went wrong", type = "error";
            if (EditProfile.FirstName != null && EditProfile.LastName != null)
            {
                if (EditProfile.FirstName == "")
                {
                    message = "first name is mandatory";
                    type = "error";
                }
                else if (EditProfile.EMail == "")
                {
                    message = "email name is mandatory";
                    type = "error";
                }
                else
                {
                    try
                    {
                        using SqlConnection connection = new(connectionString);

                        await connection.OpenAsync();
                        SqlCommand insertCommand = new("UPDATE TblUserProfile SET FirstName = @FirstName,LastName = @LastName,EMail = @EMail,Phone = @Phone,AvatarId= @AvatarId,Gender = @Gender where UserName = @UserName", connection);
                        insertCommand.Parameters.AddWithValue("@FirstName", EditProfile.FirstName.Trim());
                        insertCommand.Parameters.AddWithValue("@LastName", EditProfile.LastName.Trim());
                        insertCommand.Parameters.AddWithValue("@EMail", EditProfile.EMail.Trim());
                        insertCommand.Parameters.AddWithValue("@Phone", EditProfile.Phone.Trim());
                        insertCommand.Parameters.AddWithValue("@Gender", EditProfile.Gender.Trim());
                        insertCommand.Parameters.AddWithValue("@AvatarId", EditProfile.AvatarId);
                        insertCommand.Parameters.Add("@datejoined", SqlDbType.DateTime).Value = DateTime.Now;
                        insertCommand.Parameters.AddWithValue("@UserName", HttpContext.Session.GetString("username"));
                        await insertCommand.ExecuteNonQueryAsync();
                        message = "Changes Saved!!";
                        type = "success";


                        if (HttpContext.Session.GetString("username") != null)
                        {
                            SqlCommand AvatarCommand = new("SELECT Image FROM TblAvatarMaster WHERE Id = @urlhandle", connection);
                            AvatarCommand.Parameters.AddWithValue("@urlhandle", EditProfile.AvatarId);
                            var reader = await AvatarCommand.ExecuteReaderAsync();
                            if (await reader.ReadAsync())
                            {
                                string session_avtr = reader.GetString(0).ToString();
                                HttpContext.Session.SetString("avatar", session_avtr);
                            }
                            HttpContext.Session.SetString("first_name", EditProfile.FirstName.Trim());


                            // HttpContext.Session.SetString("role", role);
                            // HttpContext.Session.SetString("avatar", role);
                        }
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        message = "something went wrong:" + ex.Message.ToString();
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
