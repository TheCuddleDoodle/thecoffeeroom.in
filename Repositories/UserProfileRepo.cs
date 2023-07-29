using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Coffeeroom.Repositories
{
    public class UserProfileRepo : IUserProfileRepo
    {
        public string connectionString = ConfigHelper.NewConnectionString;
        public async Task<UserProfile> AddAsync(UserProfile userProfile)
        {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string checkUserQuery = "SELECT COUNT(*) FROM TblUserProfile WHERE UserName = @username OR EMail = @email";
            SqlCommand checkUserCmd = new(checkUserQuery, connection);
            checkUserCmd.Parameters.AddWithValue("@email", userProfile.EMail);
            checkUserCmd.Parameters.AddWithValue("@username", userProfile.UserName);
            int userCount = (int)await checkUserCmd.ExecuteScalarAsync();

            if (userCount == 0)
            {
                string getMaxIdQuery = "SELECT MAX(Id) FROM TblUserProfile";
                SqlCommand getMaxIdCmd = new(getMaxIdQuery, connection);
                int maxId = (int)await getMaxIdCmd.ExecuteScalarAsync();
                int newId = maxId + 1;

                string addEmailQuery = "INSERT INTO TblUserProfile (Id,FirstName,LastName,Email,Password,UserName,IsVerified,Role) VALUES (@id,@firstname,@lastname, @email,@password,@username,@isverified,@role)";
                SqlCommand addEmailCmd = new(addEmailQuery, connection);
                addEmailCmd.Parameters.AddWithValue("@id", newId);
                addEmailCmd.Parameters.AddWithValue("@firstname", userProfile.FirstName);
                addEmailCmd.Parameters.AddWithValue("@lastname", userProfile.LastName);
                addEmailCmd.Parameters.AddWithValue("@email", userProfile.EMail);
                addEmailCmd.Parameters.AddWithValue("@password", userProfile.Password);
                addEmailCmd.Parameters.AddWithValue("@username", userProfile.UserName);
                addEmailCmd.Parameters.AddWithValue("@isverified", true);
                addEmailCmd.Parameters.AddWithValue("@role", "guest");

                await addEmailCmd.ExecuteNonQueryAsync();
                Log.Information("mail added to newsletter:" + userProfile.EMail);
                return userProfile;
            }
            else
            {
                throw new Exception("email already present");
            }
            throw new NotImplementedException();
        }

        public Task<UserProfile> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserProfile>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserProfile> GetAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfile> UpdateAsync(UserProfile userProfile)
        {
            throw new NotImplementedException();
        }
    }
}
