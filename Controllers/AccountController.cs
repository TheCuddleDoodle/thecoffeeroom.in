using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Models.View;
using Coffeeroom.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;

namespace Coffeeroom.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        [HttpPost("api/account/signup")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SignUp(SignUpVals signUpVals)
        {
            var logger = new Logger();
            string body, subject;
            string connectionString = ConfigHelper.NewConnectionString;
            string message = "something working", type = "error";
            if (signUpVals.UserName != null && signUpVals.PassWord != null)
            {
                if (signUpVals.FirstName == "")
                {
                    message = "first name is mandatory";
                }
                else if (signUpVals.UserName == "")
                {
                    message = "username is mandatory";
                }
                else if (!Validators.IsAlphaNumeric(signUpVals.UserName))
                {
                    message = "only alphanumeric characters are allowed";
                }
                else if (signUpVals.EMail == "")
                {
                    message = "email is mandatory";
                }
                else if (Validators.IsValidEmail(signUpVals.EMail) == false)
                {
                    message = "invalid email format";
                }
                else if (signUpVals.PassWord.Trim() == "")
                {
                    message = "password is mandatory";
                }
                else
                {
                    try
                    {
                        string FilteredUsername = signUpVals.UserName.Trim().ToLower().ToString();
                        using SqlConnection connection = new(connectionString);
                        await connection.OpenAsync();
                        SqlCommand cmd = new("select count(*) from TblUserProfile where UserName = @inputusername or EMail = @inputemail", connection);
                        cmd.Parameters.AddWithValue("@inputusername", FilteredUsername);
                        cmd.Parameters.AddWithValue("@inputemail", signUpVals.EMail);
                        var counter = cmd.ExecuteScalar().ToString();
                        if (counter == "0")
                        {
                            string secret = StringProcessors.GenerateRandomString(10);
                            var otp = OTPGenerator.GenerateOTP(secret);
                            subject = "Verify Your Account | TheCoffeeroom";
                            try
                            {

                                body = "<h1>Hey there,</h1>" +
                                        "<p> This is for the verification of your account @TheCoffeeRoom." +
                                        "" + otp + " is your OTP which is valid for 30 minutes </p>." +
                                        "Or alternatively you can click here to verify directly:" +
                                        "<a type=\"button\" href=\"https://thecoffeeroom.in/account/verify?uzrnm=" + FilteredUsername + "&key=" + otp + "\"><b> VERIFY </b></a>";

                                int stat = Mailer.MailSignup(subject, body, signUpVals.EMail.ToString());
                                if (stat == 1)
                                {
                                    try
                                    {
                                        SqlCommand maxIdCommand = new("SELECT ISNULL(MAX(Id), 0) + 1 FROM TblUserProfile", connection);
                                        int newId = Convert.ToInt32(maxIdCommand.ExecuteScalar());
                                        cmd = new("insert into TblUserProfile (Id,FirstName,LastName,EMail,UserName,PassWord,IsActive,IsVerified,OTP,OTPTime,Role,Bio,Gender,Phone,AvatarId,DateJoined) VALUES(@Id,@firstname,@lastname,@email,@username,@password,1,0,@otp,@otptime,'user','','','',1,@datejoined)", connection);
                                        cmd.Parameters.AddWithValue("@Id", newId);
                                        cmd.Parameters.AddWithValue("@firstname", signUpVals.FirstName);
                                        cmd.Parameters.AddWithValue("@lastname", signUpVals.LastName);
                                        cmd.Parameters.AddWithValue("@email", signUpVals.EMail);
                                        cmd.Parameters.AddWithValue("@username", FilteredUsername);
                                        cmd.Parameters.AddWithValue("@password", signUpVals.PassWord);
                                        cmd.Parameters.AddWithValue("@otp", otp);
                                        cmd.Parameters.Add("@otptime", SqlDbType.DateTime).Value = DateTime.Now;
                                        cmd.Parameters.Add("@datejoined", SqlDbType.DateTime).Value = DateTime.Now;

                                        await cmd.ExecuteNonQueryAsync();
                                        message = "verification email send please verify your account";
                                        type = "success";
                                        Log.Information(signUpVals.FirstName + " registered, Email: " + signUpVals.EMail);
                                    }
                                    catch (Exception exm)
                                    {
                                        Log.Error("Error while user registration: " + exm.Message.ToString());
                                    }

                                }
                                else
                                {
                                    message = "unable to send the mail";
                                    type = "error";
                                }
                            }
                            catch (Exception ex2)
                            {
                                message = "something went wrong";
                                Log.Error(ex2.Message.ToString());
                            }
                        }
                        else
                        {
                            message = "username/email taken!!";
                            type = "error";
                        }
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        message = "something went wrong";
                        Log.Error("error while signup" + ex.Message.ToString());
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

        [HttpGet]
        [Route("/api/getavatars")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnGetAvatarListAsync()
        {
            try
            {
                List<Avatar> entries = new();
                string sql;
                string connectionString = ConfigHelper.NewConnectionString;
                using (SqlConnection connection = new(connectionString))
                {
                    await connection.OpenAsync();
                    sql = "select * from TblAvatarMaster";
                    using SqlCommand command = new(sql, connection);
                    using SqlDataReader dataReader = await command.ExecuteReaderAsync();

                    while (await dataReader.ReadAsync())
                    {
                        Avatar entry = new()
                        {
                            Id = Int32.Parse(dataReader["Id"].ToString()),
                            Title = (string)dataReader["Title"],
                            Image = (string)dataReader["Image"]
                        };
                        entries.Add(entry);
                    }
                    await connection.CloseAsync();
                }
                return new JsonResult(entries);
            }
            catch (Exception ex)
            {
                Log.Error("error in live search: " + ex.Message.ToString());
                return BadRequest("something went wrong");
            }
        }



    }
}
