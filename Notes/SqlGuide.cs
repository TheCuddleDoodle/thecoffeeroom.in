using Microsoft.Data.SqlClient;

namespace Coffeeroom.Notes
{
    public class SqlGuide
    {
        public void Resumeconnection()
        {
            using (SqlConnection connection = new SqlConnection("yourconnstrinig"))
            {
                connection.Open();

                // Query 1
                using (SqlCommand command1 = new SqlCommand("query1", connection))
                {
                    // Execute the query and process the results
                    // ...
                }

                // Query 2
                using (SqlCommand command2 = new SqlCommand("q2", connection))
                {
                    // Execute the query and process the results
                    // ...
                }

                // Query 3
                using (SqlCommand command3 = new SqlCommand("q3", connection))
                {
                    // Execute the query and process the results
                    // ...
                }
            }
        }
    }
}
