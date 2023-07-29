using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Coffeeroom.Repositories
{
    public class MailingListRepo : IMailingListRepo
    {
        public string connectionString = ConfigHelper.NewConnectionString;
        public async Task<Mail> AddAsync(Mail mail)
        {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string checkEmailQuery = "SELECT COUNT(*) FROM TblMailingList WHERE Email = @Email";
            SqlCommand checkEmailCmd = new(checkEmailQuery, connection);
            checkEmailCmd.Parameters.AddWithValue("@Email", mail.EMailId);
            int emailCount = (int)await checkEmailCmd.ExecuteScalarAsync();

            if (emailCount == 0)
            {
                string getMaxIdQuery = "SELECT MAX(Id) FROM TblMailingList";
                SqlCommand getMaxIdCmd = new(getMaxIdQuery, connection);
                int maxId = (int)await getMaxIdCmd.ExecuteScalarAsync();
                int newId = maxId + 1;

                string addEmailQuery = "INSERT INTO TblMailingList (Id, Email,Origin,DateAdded) VALUES (@id, @email,@origin,@dateadded)";
                SqlCommand addEmailCmd = new(addEmailQuery, connection);
                addEmailCmd.Parameters.AddWithValue("id", newId);
                addEmailCmd.Parameters.AddWithValue("@email", mail.EMailId);
                addEmailCmd.Parameters.AddWithValue("@origin", mail.Origin);
                addEmailCmd.Parameters.AddWithValue("@dateadded", DateTime.Now);
                await addEmailCmd.ExecuteNonQueryAsync();
                Log.Information("mail added to newsletter:" + mail.EMailId);
                return mail;
            }
            else
            {
                throw new Exception("email already present");
            }
            throw new NotImplementedException();
        }

        public Task<Mail> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Mail>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Mail> GetAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<Mail> UpdateAsync(Mail mail)
        {
            throw new NotImplementedException();
        }
    }
}
