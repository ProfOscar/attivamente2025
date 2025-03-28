using Microsoft.Data.SqlClient;

namespace AttivaMente.Data
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public SqlDataReader ExecuteReader(string query)
        {
            var connection = GetConnection();
            var command = new SqlCommand(query, connection);
            connection.Open();
            return command.ExecuteReader();
        }
    }
}