using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories
{
    public class ProdutoRepository
    {
        private SqliteConnection ObterConexao()
        {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        // C - CREATE (Blindado)
        public void Inserir(Produto produto)
        {
            using var conexao = ObterConexao();

            // Estratégia de Integridade: O estoque sempre nasce zerado.
            // A carga inicial deve ser feita via rotina de Movimentação.
            string sql = @"
                INSERT INTO Produtos (CodigoBarras, Nome, Descricao, Marca, Categoria, PrecoCusto, PrecoVenda, EstoqueAtual, EstoqueMinimo) 
                VALUES (@CodigoBarras, @Nome, @Descricao, @Marca, @Categoria, @PrecoCusto, @PrecoVenda, 0, @EstoqueMinimo)";

            conexao.Execute(sql, produto);
        }

        // R - READ (Todos)
        public List<Produto> BuscarTodos()
        {
            using var conexao = ObterConexao();
            // A função CAST força o SQLite a devolver um tipo compatível com o decimal do C#
            string sql = @"
                SELECT Id, CodigoBarras, Nome, Descricao, Marca, Categoria, 
                       CAST(PrecoCusto AS REAL) AS PrecoCusto, 
                       CAST(PrecoVenda AS REAL) AS PrecoVenda, 
                       EstoqueAtual, EstoqueMinimo 
                FROM Produtos ORDER BY Nome";

            return conexao.Query<Produto>(sql).ToList();
        }

        // R - READ (Por ID)
        public Produto BuscarPorId(int id)
        {
            using var conexao = ObterConexao();
            string sql = @"
                SELECT Id, CodigoBarras, Nome, Descricao, Marca, Categoria, 
                       CAST(PrecoCusto AS REAL) AS PrecoCusto, 
                       CAST(PrecoVenda AS REAL) AS PrecoVenda, 
                       EstoqueAtual, EstoqueMinimo 
                FROM Produtos WHERE Id = @Id";

            return conexao.QueryFirstOrDefault<Produto>(sql, new { Id = id });
        }

        // R - READ ESTRATÉGICO (Alerta de Reposição de Estoque)
        public List<Produto> BuscarProdutosAlertaEstoque()
        {
            using var conexao = ObterConexao();
            string sql = @"
                SELECT Id, CodigoBarras, Nome, Descricao, Marca, Categoria, 
                       CAST(PrecoCusto AS REAL) AS PrecoCusto, 
                       CAST(PrecoVenda AS REAL) AS PrecoVenda, 
                       EstoqueAtual, EstoqueMinimo 
                FROM Produtos 
                WHERE EstoqueAtual <= EstoqueMinimo ORDER BY EstoqueAtual ASC";

            return conexao.Query<Produto>(sql).ToList();
        }

        // U - UPDATE (Blindado)
        public void Atualizar(Produto produto)
        {
            using var conexao = ObterConexao();

            // Estratégia de Integridade: EstoqueAtual foi omitido do SET.
            // Modificações no catálogo não podem alterar o saldo físico.
            string sql = @"
                UPDATE Produtos 
                SET CodigoBarras = @CodigoBarras, Nome = @Nome, Descricao = @Descricao, 
                    Marca = @Marca, Categoria = @Categoria, PrecoCusto = @PrecoCusto, 
                    PrecoVenda = @PrecoVenda, EstoqueMinimo = @EstoqueMinimo 
                WHERE Id = @Id";

            conexao.Execute(sql, produto);
        }

        // D - DELETE
        public void Excluir(int id)
        {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Produtos WHERE Id = @Id";

            conexao.Execute(sql, new { Id = id });
        }
    }
}