using Dapper;
using PetShopCare.Database;
using PetShopCare.Models;
using Microsoft.Data.Sqlite;
using System;

namespace PetShopCare.Repositories {
    public class EstoqueRepository {
        private SqliteConnection ObterConexao() => new SqliteConnection(DatabaseConfig.ConnectionString);

        public void RegistrarMovimentacao(EstoqueMovimentacao mov) {
            using var conexao = ObterConexao();
            conexao.Open();
            using var transacao = conexao.BeginTransaction();

            try {
                // 1. Grava o histórico (O Porquê)
                string sqlMov = @"
                    INSERT INTO EstoqueMovimentacao (ProdutoId, Quantidade, TipoMovimentacao, Origem, DataMovimentacao, UsuarioId)
                    VALUES (@ProdutoId, @Quantidade, @TipoMovimentacao, @Origem, @DataMovimentacao, @UsuarioId)";

                conexao.Execute(sqlMov, mov, transacao);

                // 2. Atualiza o saldo real (O Resultado)
                // Se for Entrada, soma. Se for Saída, subtrai.
                string operacao = mov.TipoMovimentacao == "Entrada" ? "+" : "-";
                string sqlUpdate = $@"
                    UPDATE Produtos 
                    SET EstoqueAtual = EstoqueAtual {operacao} @Quantidade 
                    WHERE Id = @ProdutoId";

                conexao.Execute(sqlUpdate, new { mov.Quantidade, mov.ProdutoId }, transacao);

                transacao.Commit();
            }
            catch {
                transacao.Rollback();
                throw;
            }
        }
    }
}