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

                string sqlUsuarios = @"
                    CREATE TABLE IF NOT EXISTS Usuarios (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nome TEXT NOT NULL,
                        Usuario TEXT NOT NULL UNIQUE,
                        SenhaHash TEXT NOT NULL,
                        Cargo TEXT,
                        Status TEXT,
                        DataCadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Perfil INTEGER NOT NULL DEFAULT 1
                    );";

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

                string sqlServicos = @"
                    CREATE TABLE IF NOT EXISTS Servicos (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nome TEXT NOT NULL,
                        Preco NUMERIC NOT NULL,
                        TempoEstimadoMinutos INTEGER NOT NULL
                    );";

                string sqlOrdensServico = @"
                    CREATE TABLE IF NOT EXISTS OrdensServico (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClienteId INTEGER NOT NULL,
                        PetId INTEGER NOT NULL,
                        ServicoId INTEGER NOT NULL,
                        UsuarioResponsavelId INTEGER NOT NULL,
                        DataHoraAgendamento TEXT NOT NULL,
                        Valor NUMERIC NOT NULL,
                        Status TEXT NOT NULL,
                        Observacoes TEXT,
                        FOREIGN KEY (PetId) REFERENCES Pets(Id),
                        FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
                        FOREIGN KEY (ServicoId) REFERENCES Servicos(Id)
                    );";

                string sqlVendas = @"
                    CREATE TABLE IF NOT EXISTS Vendas (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClienteId INTEGER,
                        UsuarioId INTEGER NOT NULL,
                        DataVenda TEXT NOT NULL,
                        ValorTotal NUMERIC NOT NULL,
                        Desconto NUMERIC DEFAULT 0,
                        FormaPagamento TEXT,
                        Status TEXT,
                        FOREIGN KEY(ClienteId) REFERENCES Clientes(Id)
                    );";

                string sqlVendaItens = @"
                    CREATE TABLE IF NOT EXISTS VendaItens (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        VendaId INTEGER NOT NULL,
                        ProdutoId INTEGER NOT NULL,
                        Quantidade INTEGER NOT NULL,
                        PrecoUnitario NUMERIC NOT NULL,
                        Subtotal NUMERIC NOT NULL,
                        FOREIGN KEY(VendaId) REFERENCES Vendas(Id) ON DELETE CASCADE,
                        FOREIGN KEY(ProdutoId) REFERENCES Produtos(Id)
                    );";

                // Executa a criação da tabela Usuarios
                using (var commandUsuarios = new SqliteCommand(sqlUsuarios, connection))
                {
                    commandUsuarios.ExecuteNonQuery();
                }

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

                // Executa a criação da tabela Servicos
                using (var commandServicos = new SqliteCommand(sqlServicos, connection))
                {
                    commandServicos.ExecuteNonQuery();
                }

                // Executa a criação da tabela OrdensServico
                using (var commandOrdensServico = new SqliteCommand(sqlOrdensServico, connection))
                {
                    commandOrdensServico.ExecuteNonQuery();
                }

                // Executa a criação da tabela Vendas
                using (var commandVendas = new SqliteCommand(sqlVendas, connection))
                {
                    commandVendas.ExecuteNonQuery();
                }

                // Executa a criação da tabela VendaItens
                using (var commandVendaItens = new SqliteCommand(sqlVendaItens, connection))
                {
                    commandVendaItens.ExecuteNonQuery();
                }

                // =========================================================================
                // SEEDS AUTOMÁTICOS
                // =========================================================================

                // SEED: Cria o usuário Admin automaticamente se não existir nenhum
                string checkUsuarios = "SELECT COUNT(*) FROM Usuarios";
                using (var commandCheckUsuarios = new SqliteCommand(checkUsuarios, connection))
                {
                    long count = (long)commandCheckUsuarios.ExecuteScalar();
                    if (count == 0)
                    {
                        string seedUsuarios = @"
                            INSERT INTO Usuarios (Nome, Usuario, SenhaHash, Cargo, Status, Perfil) 
                            VALUES ('Administrador do Sistema', 'admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Gerente', 'Ativo', 1);";

                        using (var commandSeedUsuarios = new SqliteCommand(seedUsuarios, connection))
                        {
                            commandSeedUsuarios.ExecuteNonQuery();
                        }
                    }
                }

                // SEED: Serviços
                string checkServicos = "SELECT COUNT(*) FROM Servicos";
                using (var commandCheck = new SqliteCommand(checkServicos, connection))
                {
                    long count = (long)commandCheck.ExecuteScalar();
                    if (count == 0)
                    {
                        string seedServicos = @"
                            INSERT INTO Servicos (Nome, Preco, TempoEstimadoMinutos) VALUES 
                            ('Banho - Pequeno Porte', 50.00, 30),
                            ('Banho - Grande Porte', 80.00, 50),
                            ('Tosa Higiênica', 40.00, 30),
                            ('Banho e Tosa Completa', 120.00, 90),
                            ('Consulta Veterinária Clínica', 150.00, 45);";

                        using (var commandSeed = new SqliteCommand(seedServicos, connection))
                        {
                            commandSeed.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}