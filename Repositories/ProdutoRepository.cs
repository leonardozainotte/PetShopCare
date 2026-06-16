using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories {
    public class ProdutoRepository {
        private SqliteConnection ObterConexao() {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        // C - CREATE
        public void Inserir(Produto produto) {
            using var conexao = ObterConexao();
            string sql = @"
                INSERT INTO Produtos (CodigoBarras, Nome, Descricao, Marca, Categoria, PrecoCusto, PrecoVenda, EstoqueAtual, EstoqueMinimo) 
                VALUES (@CodigoBarras, @Nome, @Descricao, @Marca, @Categoria, @PrecoCusto, @PrecoVenda, @EstoqueAtual, @EstoqueMinimo)";

            conexao.Execute(sql, produto);
        }

        // R - READ (Todos)
        public List<Produto> BuscarTodos() {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Produtos ORDER BY Nome";

            return conexao.Query<Produto>(sql).ToList();
        }

        // R - READ (Por ID)
        public Produto BuscarPorId(int id) {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Produtos WHERE Id = @Id";

            return conexao.QueryFirstOrDefault<Produto>(sql, new { Id = id });
        }

        // R - READ ESTRATÉGICO (Alerta de Reposição de Estoque)
        public List<Produto> BuscarProdutosAlertaEstoque() {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Produtos WHERE EstoqueAtual <= EstoqueMinimo ORDER BY EstoqueAtual ASC";

            return conexao.Query<Produto>(sql).ToList();
        }

        // U - UPDATE
        public void Atualizar(Produto produto) {
            using var conexao = ObterConexao();
            string sql = @"
                UPDATE Produtos 
                SET CodigoBarras = @CodigoBarras, Nome = @Nome, Descricao = @Descricao, 
                    Marca = @Marca, Categoria = @Categoria, PrecoCusto = @PrecoCusto, 
                    PrecoVenda = @PrecoVenda, EstoqueAtual = @EstoqueAtual, EstoqueMinimo = @EstoqueMinimo 
                WHERE Id = @Id";

            conexao.Execute(sql, produto);
        }

        // D - DELETE
        public void Excluir(int id) {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Produtos WHERE Id = @Id";

            conexao.Execute(sql, new { Id = id });
        }
    }
}