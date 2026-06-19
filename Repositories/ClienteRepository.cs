using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories
{
    public class ClienteRepository
    {
        // O encapsulamento da conexão garante o isolamento da infraestrutura
        private SqliteConnection ObterConexao()
        {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        // C - CREATE (Inserir)
        public void Inserir(Cliente cliente)
        {
            using var conexao = ObterConexao();
            // A query agora contempla os campos da modelagem atualizada
            string sql = @"
                INSERT INTO Clientes (Nome, CPF, Endereco, Telefone, Email) 
                VALUES (@Nome, @CPF, @Endereco, @Telefone, @Email)";

            conexao.Execute(sql, cliente);
        }

        // R - READ (Buscar Todos)
        public List<Cliente> BuscarTodos()
        {
            using var conexao = ObterConexao();
            // O Dapper processa o DateTime interno e os campos nulos automaticamente
            string sql = "SELECT * FROM Clientes ORDER BY Nome";

            return conexao.Query<Cliente>(sql).ToList();
        }

        // R - READ (Buscar por ID)
        public Cliente BuscarPorId(int id)
        {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Clientes WHERE Id = @Id";

            return conexao.QueryFirstOrDefault<Cliente>(sql, new { Id = id });
        }

        // U - UPDATE (Atualizar)
        public void Atualizar(Cliente cliente)
        {
            using var conexao = ObterConexao();
            string sql = @"
                UPDATE Clientes 
                SET Nome = @Nome, CPF = @CPF, Endereco = @Endereco, 
                    Telefone = @Telefone, Email = @Email 
                WHERE Id = @Id";

            conexao.Execute(sql, cliente);
        }

        // D - DELETE (Excluir)
        public void Excluir(int id)
        {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Clientes WHERE Id = @Id";

            conexao.Execute(sql, new { Id = id });
        }
    }
}