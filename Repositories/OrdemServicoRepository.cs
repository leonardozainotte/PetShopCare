using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories {
    public class OrdemServicoRepository {
        private SqliteConnection ObterConexao() {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        public void Inserir(OrdemServico os) {
            using var conexao = ObterConexao();
            string sql = @"
                INSERT INTO OrdensServico (ClienteId, PetId, ServicoId, UsuarioResponsavelId, DataHoraAgendamento, Valor, Status, Observacoes) 
                VALUES (@ClienteId, @PetId, @ServicoId, @UsuarioResponsavelId, @DataHoraAgendamento, @Valor, @Status, @Observacoes)";
            conexao.Execute(sql, os);
        }

        public List<OrdemServico> BuscarAgendaDoDia() {
            using var conexao = ObterConexao();
            // Busca os agendamentos ordenados por horário do dia atual em diante
            string sql = "SELECT * FROM OrdensServico WHERE Status != 'Cancelado' ORDER BY DataHoraAgendamento ASC";
            return conexao.Query<OrdemServico>(sql).ToList();
        }

        public void AtualizarStatus(int osId, string novoStatus) {
            using var conexao = ObterConexao();
            string sql = "UPDATE OrdensServico SET Status = @Status WHERE Id = @Id";
            conexao.Execute(sql, new { Status = novoStatus, Id = osId });
        }
    }
}