using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories
{
    public class ServicoRepository
    {
        private SqliteConnection ObterConexao()
        {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        public List<Servico> BuscarTodos()
        {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Servicos ORDER BY Nome";
            return conexao.Query<Servico>(sql).ToList();
        }

        public void Adicionar(Servico servico)
        {
            using var conexao = ObterConexao();
            string sql = @"
                INSERT INTO Servicos (Nome, Preco, TempoEstimadoMinutos) 
                VALUES (@Nome, @Preco, @TempoEstimadoMinutos)";
            conexao.Execute(sql, servico);
        }

        public void Atualizar(Servico servico)
        {
            using var conexao = ObterConexao();
            string sql = @"
                UPDATE Servicos 
                SET Nome = @Nome, 
                    Preco = @Preco, 
                    TempoEstimadoMinutos = @TempoEstimadoMinutos 
                WHERE Id = @Id";
            conexao.Execute(sql, servico);
        }

        public void Excluir(int id)
        {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Servicos WHERE Id = @Id";
            conexao.Execute(sql, new { Id = id });
        }
    }
}