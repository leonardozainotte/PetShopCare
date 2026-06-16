using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories {
    public class EstoqueMovimentacaoRepository {
        private SqliteConnection ObterConexao() {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        public void Inserir(EstoqueMovimentacao movimentacao) {
            using var conexao = ObterConexao();
            string sql = @"
                INSERT INTO EstoqueMovimentacao (ProdutoId, Quantidade, TipoMovimentacao, Origem, UsuarioId) 
                VALUES (@ProdutoId, @Quantidade, @TipoMovimentacao, @Origem, @UsuarioId)";
            conexao.Execute(sql, movimentacao);
        }

        public List<EstoqueMovimentacao> BuscarPorProdutoId(int produtoId) {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM EstoqueMovimentacao WHERE ProdutoId = @ProdutoId ORDER BY DataMovimentacao DESC";
            return conexao.Query<EstoqueMovimentacao>(sql, new { ProdutoId = produtoId }).ToList();
        }
    }
}