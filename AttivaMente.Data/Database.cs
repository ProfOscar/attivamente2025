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
                string ldfPath = $"{dbFolder}\\{dbName}_log.ldf";

                // Questo blocco try...catch è indispensabile e va mantenuto
                // perché evita un problema in caso di cancellazione manuale
                // del dbf dal file system
                try { 
                    using (var connection = new SqlConnection(@"Server=(localDB)\MSSQLLocalDB;Integrated Security=true"))
                    {

                        string sql = $@"
                        IF DB_ID('{dbName}') IS NOT NULL
                        BEGIN
                            ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                            DROP DATABASE [{dbName}];
                        END";
                        connection.Open();
                        using var commandDrop = new SqlCommand(sql, connection);
                        commandDrop.ExecuteNonQuery();
                    }
                } catch { }

                using (var connection = new SqlConnection(@"Server=(localDB)\MSSQLLocalDB;Integrated Security=true"))
                {
                    string sql = $@"CREATE DATABASE [{dbName}]
                    ON PRIMARY (
                        NAME = {dbName}_Data,
                        FILENAME = '{dbPath}'
                    )
                    LOG ON (
                        NAME = {dbName}_Log,
                        FILENAME = '{ldfPath}'
                    )";
                    connection.Open();
                    using var commandCreate = new SqlCommand(sql, connection);
                    commandCreate.ExecuteNonQuery();
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

                // Eseguo lo script per creare le tabelle sul db appena creato
                ExecuteScript(schemaPath);
            }
        }

        public void InitialDataSeed(string dbPath, string seedDataPath)
        {
            // Controlla se ci non ci sono dati (COUNT = 0)
            // nelle tabelle Ruoli e Utenti; in quel caso lancia lo script
            string sql = @"SELECT MAX(A.C) FROM 
                            (SELECT COUNT(*) as C FROM Ruoli 
                            UNION ALL 
                            SELECT COUNT(*) AS C FROM Utenti) 
                            A";
            int n = (int)ExecuteScalar(sql);
            if (n == 0)
                ExecuteScript(seedDataPath);
        }
    }
}