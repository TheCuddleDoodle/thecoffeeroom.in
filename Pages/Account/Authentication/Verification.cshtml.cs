using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Coffeeroom.Pages.Account
{
    public class VerificationModel : PageModel
    {
        public UserProfile UserDetailsDisplay { get; set; }
        public string Uzrnm { get; set; }
        public string Key { get; set; }
        public string Otp { get; set; }
        public string Stat { get; set; }
        public async Task OnGetAsync()
        {
            Stat = "init";
            Uzrnm = HttpContext.Request.Query["Uzrnm"].ToString();
            Key = HttpContext.Request.Query["Key"].ToString();
            // Otp = Cryptolib.Decrypt(Key, ConfigHelper.CryptoKey.ToString() );
            // string frombase64 = Otp.Replace('_', '/');
            string connectionString = ConfigHelper.NewConnectionString;
            using SqlConnection connection = new(connectionString);
            connection.Open();
            SqlCommand checkcmd = new("select * from TblUserProfile where Username ='" + Uzrnm + "' and OTP ='" + Key + "' and IsVerified = 0 ", connection);
            SqlDataReader dataReader = await checkcmd.ExecuteReaderAsync();
            if (dataReader.Read())
            {
                UserDetailsDisplay = new UserProfile()
                {
                    FirstName = dataReader.GetString(1)
                };
                await dataReader.CloseAsync();
                SqlCommand activatecmd = new("UPDATE TblUserProfile SET IsVerified = 1 where UserName ='" + Uzrnm + "' ", connection);
                await activatecmd.ExecuteNonQueryAsync();
                Stat = "valid";
            }
            else
            {
                Stat = "invalid";
            }
            await connection.CloseAsync();
        }
    }
}
