using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Coffeeroom.Repositories
{
    public class PassResetTokenRepo : IPassResetTokenRepo
    {
        public string connectionString = ConfigHelper.NewConnectionString;
        public async Task<PasswordToken> AddToken(PasswordToken passwordToken)
        {
            int maxId = 1, newId = 1;
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            string checkUserQuery = "SELECT COUNT(*) FROM TblPasswordReset WHERE UserId = @userid";
            SqlCommand checkUserCmd = new(checkUserQuery, connection);
            checkUserCmd.Parameters.AddWithValue("@userid", passwordToken.UserId);
            int userCount = (int)await checkUserCmd.ExecuteScalarAsync();

            if (userCount != 0)
            {
                string delQuery = "DELETE FROM TblPasswordReset WHERE UserId = @userid";
                SqlCommand delCmd = new(delQuery, connection);
                delCmd.Parameters.AddWithValue("@userid", passwordToken.UserId);
                delCmd.ExecuteNonQuery();
            }
           

                string getMaxIdQuery = "SELECT MAX(Id) FROM TblPasswordReset";
                SqlCommand getMaxIdCmd = new(getMaxIdQuery, connection);
                if (await getMaxIdCmd.ExecuteScalarAsync() != DBNull.Value)
                {
                maxId = (int)await getMaxIdCmd.ExecuteScalarAsync();
                newId = maxId + 1;
                
                }
                else
                {
                    maxId = 1;
                }
                string addEmailQuery = "INSERT INTO TblPasswordReset (Id,UserId,Token,DateAdded,IsValid) VALUES (@id,@userid,@token,@dateadded,@isvalid)";
                SqlCommand addEmailCmd = new(addEmailQuery, connection);
                addEmailCmd.Parameters.AddWithValue("@id", newId);
                addEmailCmd.Parameters.AddWithValue("@userid", passwordToken.UserId);
                addEmailCmd.Parameters.AddWithValue("@token", passwordToken.Token);
                addEmailCmd.Parameters.AddWithValue("@dateadded", passwordToken.DateAdded);
                addEmailCmd.Parameters.AddWithValue("@isvalid", passwordToken.IsValid);

                await addEmailCmd.ExecuteNonQueryAsync();
                Log.Information("password reset request from userId:" + passwordToken.UserId);
                return passwordToken;
            
           
            throw new NotImplementedException();
           
        }

        public Task ValidateToken(PasswordToken passwordToken)
        {
            throw new NotImplementedException();
        }
    }
}
