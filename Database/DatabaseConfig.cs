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

                // --- INÍCIO DA ETAPA 9: PRODUTOS E ESTOQUE ---
                string sqlProdutos = @"
                    CREATE TABLE IF NOT EXISTS Produtos (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CodigoBarras TEXT,
                        Nome TEXT NOT NULL,
                        Descricao TEXT,
                        Marca TEXT,
                        Categoria TEXT,
                        PrecoCusto NUMERIC NOT NULL,
                        PrecoVenda NUMERIC NOT NULL,
                        EstoqueAtual INTEGER NOT NULL DEFAULT 0,
                        EstoqueMinimo INTEGER NOT NULL DEFAULT 0
                    );";

                string sqlMovimentacao = @"
                    CREATE TABLE IF NOT EXISTS EstoqueMovimentacao (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProdutoId INTEGER NOT NULL,
                        Quantidade INTEGER NOT NULL,
                        TipoMovimentacao TEXT NOT NULL,
                        Origem TEXT,
                        DataMovimentacao TEXT NOT NULL,
                        UsuarioId INTEGER NOT NULL,
                        FOREIGN KEY(ProdutoId) REFERENCES Produtos(Id)
                    );";
                // --- FIM DA ETAPA 9 ---


                // Executa a criação da tabela Clientes
                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Executa a criação da tabela Pets
                using (var commandPets = new SqliteCommand(sqlPets, connection))
                {
                    commandPets.ExecuteNonQuery();
                }

                // Executa a criação da tabela Produtos
                using (var commandProdutos = new SqliteCommand(sqlProdutos, connection))
                {
                    commandProdutos.ExecuteNonQuery();
                }

                // Executa a criação da tabela EstoqueMovimentacao
                using (var commandMovimentacao = new SqliteCommand(sqlMovimentacao, connection))
                {
                    commandMovimentacao.ExecuteNonQuery();
                }
            }
        }
    }
}