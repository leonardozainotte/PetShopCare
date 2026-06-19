using Microsoft.Data.Sqlite;
using System.IO;

namespace PetShopCare.Database
{
    public static class DatabaseConfig
    {
        private static string _databaseFile = "PetShopCare.db";
        public static string ConnectionString => $"Data Source={_databaseFile}";

        public static void InitializeDatabase()
        {
            if (!File.Exists(_databaseFile))
            {
                File.Create(_databaseFile).Dispose();
            }

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Clientes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nome TEXT NOT NULL,
                        CPF TEXT UNIQUE NOT NULL,
                        Endereco TEXT,
                        Telefone TEXT,
                        Email TEXT,
                    );";

                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}