using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories
{
    public class PetRepository
    {
        private SqliteConnection ObterConexao()
        {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        // C - CREATE (Inserir)
        public void Inserir(Pet pet)
        {
            using var conexao = ObterConexao();
            string sql = @"
                INSERT INTO Pets (ClienteId, Nome, Especie, Raca, Sexo, DataNascimento, Peso, Cor, Observacoes) 
                VALUES (@ClienteId, @Nome, @Especie, @Raca, @Sexo, @DataNascimento, @Peso, @Cor, @Observacoes)";

            conexao.Execute(sql, pet);
        }

        // R - READ (Buscar Todos)
        public List<Pet> BuscarTodos()
        {
            using var conexao = ObterConexao();
            string sql = @"
                SELECT p.*, c.Nome AS ClienteNome 
                FROM Pets p
                INNER JOIN Clientes c ON p.ClienteId = c.Id
                ORDER BY p.Nome";

            return conexao.Query<Pet>(sql).ToList();
        }

        // R - READ (Buscar por ID do Pet)
        public Pet BuscarPorId(int id)
        {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Pets WHERE Id = @Id";

            return conexao.QueryFirstOrDefault<Pet>(sql, new { Id = id });
        }

        // R - READ ESTRATÉGICO (Buscar todos os Pets de um Tutor específico)
        public List<Pet> BuscarPorClienteId(int clienteId)
        {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Pets WHERE ClienteId = @ClienteId ORDER BY Nome";

            return conexao.Query<Pet>(sql, new { ClienteId = clienteId }).ToList();
        }

        // U - UPDATE (Atualizar)
        public void Atualizar(Pet pet)
        {
            using var conexao = ObterConexao();
            string sql = @"
                UPDATE Pets 
                SET ClienteId = @ClienteId, Nome = @Nome, Especie = @Especie, Raca = @Raca, 
                    Sexo = @Sexo, DataNascimento = @DataNascimento, Peso = @Peso, 
                    Cor = @Cor, Observacoes = @Observacoes 
                WHERE Id = @Id";

            conexao.Execute(sql, pet);
        }

        // D - DELETE (Excluir)
        public void Excluir(int id)
        {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Pets WHERE Id = @Id";

            conexao.Execute(sql, new { Id = id });
        }

        // D - DELETE (Excluir em Cascata)
        public void ExcluirPorClienteId(int clienteId)
        {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Pets WHERE ClienteId = @ClienteId";

            conexao.Execute(sql, new { ClienteId = clienteId });
        }
    }
}