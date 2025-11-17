using Microsoft.Data.SqlClient;
using System.Data;

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

        public SqlDataReader ExecuteReader(string query, SqlParameter[]? parameters = null)
        {
            var connection = GetConnection();
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters);
            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public int ExecuteNonQuery(string sql, SqlParameter[]? parameters = null)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(sql, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql, SqlParameter[]? parameters = null)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(sql, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters);
            connection.Open();
            return command.ExecuteScalar();
        }

        public void ExecuteScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException("Script SQL non trovato", scriptPath);

            string script = File.ReadAllText(scriptPath);
            ExecuteNonQuery(script);
        }

        private void CreateDatabase(string dbPath)
        {
            string dbName = Path.GetFileName(dbPath).Replace(".mdf", "");
            string dbFolder = Path.GetDirectoryName(dbPath)!;

            try
            {
                using (var connection = new SqlConnection(@"Server=(localDB)\MSSQLLocalDB;Integrated Security=true"))
                {
                    string ldfPath = $"{dbFolder}\\{dbName}_log.ldf";
                    string sql = $@"
                    CREATE DATABASE [{dbName}]
                    ON PRIMARY (
                        NAME = {dbName}_Data,
                        FILENAME = '{dbPath}'
                    )
                    LOG ON (
                        NAME = {dbName}_Log,
                        FILENAME = '{ldfPath}'
                    )";

                    connection.Open();
                    using var command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void EnsureDatabaseCreated(string dbPath, string schemaPath)
        {
            if (!File.Exists(dbPath))
            {
                var folder = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder!);

                // Creo il db
                CreateDatabase(dbPath);

                // Eseguo lo script per lo schema sul db appena creato
                ExecuteScript(schemaPath);
            }
        }
    }
}