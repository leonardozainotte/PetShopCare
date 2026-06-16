using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories {
    public class VendaRepository {
        private SqliteConnection ObterConexao() {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        // Processo complexo de venda encapsulado em uma única transação atômica
        public void ProcessarVenda(Venda venda, List<ItemVenda> itens) {
            using var conexao = ObterConexao();
            conexao.Open();
            using var transacao = conexao.BeginTransaction();

            try {
                // 1. Inserir a Venda Principal
                string sqlVenda = @"
                    INSERT INTO Vendas (UsuarioId, ClienteId, ValorTotal, Desconto, FormaPagamento, Status) 
                    VALUES (@UsuarioId, @ClienteId, @ValorTotal, @Desconto, @FormaPagamento, @Status);
                    SELECT last_insert_rowid();"; // Retorna o ID gerado para esta venda no SQLite

                int vendaId = conexao.QuerySingle<int>(sqlVenda, venda, transacao);

                foreach (var item in itens) {
                    item.VendaId = vendaId;

                    // 2. Inserir o Item da Venda
                    string sqlItem = @"
                        INSERT INTO ItensVenda (VendaId, ProdutoId, Quantidade, PrecoUnitario, Subtotal) 
                        VALUES (@VendaId, @ProdutoId, @Quantidade, @PrecoUnitario, @Subtotal)";
                    conexao.Execute(sqlItem, item, transacao);

                    // 3. Atualizar o Estoque Atual do Produto correspondente
                    string sqlEstoque = @"
                        UPDATE Produtos 
                        SET EstoqueAtual = EstoqueAtual - @Quantidade 
                        WHERE Id = @ProdutoId";
                    conexao.Execute(sqlEstoque, new { Quantidade = item.Quantidade, ProdutoId = item.ProdutoId }, transacao);

                    // 4. Registrar o histórico na tabela de movimentação de estoque
                    string sqlMovimentacao = @"
                        INSERT INTO EstoqueMovimentacao (ProdutoId, Quantidade, TipoMovimentacao, Origem, UsuarioId) 
                        VALUES (@ProdutoId, @Quantidade, 'Saida', 'Venda ERP', @UsuarioId)";
                    conexao.Execute(sqlMovimentacao, new { ProdutoId = item.ProdutoId, Quantidade = item.Quantidade, UsuarioId = venda.UsuarioId }, transacao);
                }

                // Se todas as etapas executaram sem erros, confirma as alterações no arquivo físico
                transacao.Commit();
            }
            catch (Exception) {
                // Se qualquer comando falhar, desfaz todas as inserções e alterações da memória
                transacao.Rollback();
                throw;
            }
        }

        public List<Venda> BuscarTodas() {
            using var conexao = ObterConexao();
            string sql = "SELECT * FROM Vendas ORDER BY DataVenda DESC";
            return conexao.Query<Venda>(sql).ToList();
        }
    }
}