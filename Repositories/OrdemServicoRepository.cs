using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories
{
    public class OrdemServicoRepository
    {
        private SqliteConnection ObterConexao()
        {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        public void Agendar(OrdemServico ordem)
        {
            using var conexao = ObterConexao();
            string sql = @"
                INSERT INTO OrdensServico (ClienteId, PetId, ServicoId, UsuarioResponsavelId, DataHoraAgendamento, Valor, Status, Observacoes) 
                VALUES (@ClienteId, @PetId, @ServicoId, @UsuarioResponsavelId, @DataHoraAgendamento, @Valor, @Status, @Observacoes)";

            conexao.Execute(sql, ordem);
        }

        public List<OrdemServico> BuscarAgendamentosDoDia(string dataISO)
        {
            using var conexao = ObterConexao();
            // Filtra os agendamentos comparando apenas a data no padrão do SQLite
            string sql = @"
                SELECT * FROM OrdensServico 
                WHERE date(DataHoraAgendamento) = date(@DataISO) 
                ORDER BY DataHoraAgendamento";

            return conexao.Query<OrdemServico>(sql, new { DataISO = dataISO }).ToList();
        }

        public void AtualizarStatus(int id, string novoStatus)
        {
            using var conexao = ObterConexao();
            string sql = "UPDATE OrdensServico SET Status = @NovoStatus WHERE Id = @Id";
            conexao.Execute(sql, new { NovoStatus = novoStatus, Id = id });
        }
    }
}