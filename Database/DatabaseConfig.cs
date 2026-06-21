using Microsoft.Data.Sqlite;
using System.IO;

namespace PetShopCare.Database {
    public static class DatabaseConfig {
        private static string _databaseFile = "PetShopCare.db";
        public static string ConnectionString => $"Data Source={_databaseFile}";

        public static void InitializeDatabase() {
            if (!File.Exists(_databaseFile)) {
                File.Create(_databaseFile).Dispose();
            }

            using (var connection = new SqliteConnection(ConnectionString)) {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Clientes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nome TEXT NOT NULL,
                        CPF TEXT UNIQUE NOT NULL,
                        Endereco TEXT,
                        Telefone TEXT,
                        Email TEXT
                    );";

                string sqlPets = @"
                    CREATE TABLE IF NOT EXISTS Pets (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClienteId INTEGER NOT NULL,
                        Nome TEXT NOT NULL,
                        Especie TEXT,
                        Raca TEXT,
                        Sexo TEXT,
                        DataNascimento TEXT,
                        Peso REAL,
                        Cor TEXT,
                        Observacoes TEXT,
                        FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE
                    );";

                // Executa a criação da tabela Clientes
                using (var command = new SqliteCommand(createTableQuery, connection)) {
                    command.ExecuteNonQuery();
                }

                // CORREÇÃO: Executa a criação da tabela Pets
                using (var commandPets = new SqliteCommand(sqlPets, connection)) {
                    commandPets.ExecuteNonQuery();
                }
            }
        }
    }
}