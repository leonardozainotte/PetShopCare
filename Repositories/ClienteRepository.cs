using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories {
	public class ClienteRepository {
		// Método centralizado para abrir a conexão usando nossa classe de configuração
		private SqliteConnection ObterConexao() {
			return new SqliteConnection(DatabaseConfig.ConnectionString);
		}

		// C - CREATE (Inserir)
		public void Inserir(Cliente cliente) {
			using var conexao = ObterConexao();
			string sql = @"
                INSERT INTO Clientes (Nome, CPF, Endereco, Telefone, Email) 
                VALUES (@Nome, @CPF, @Endereco, @Telefone, @Email)";

			// O Dapper pega o objeto 'cliente' e substitui os @ pelos valores das propriedades automaticamente
			conexao.Execute(sql, cliente);
		}

		// R - READ (Buscar Todos)
		public List<Cliente> BuscarTodos() {
			using var conexao = ObterConexao();
			string sql = "SELECT * FROM Clientes ORDER BY Nome";

			// O Dapper executa o SELECT e já devolve uma lista de objetos Cliente pronta
			return conexao.Query<Cliente>(sql).ToList();
		}

		// R - READ (Buscar por ID)
		public Cliente BuscarPorId(int id) {
			using var conexao = ObterConexao();
			string sql = "SELECT * FROM Clientes WHERE Id = @Id";

			return conexao.QueryFirstOrDefault<Cliente>(sql, new { Id = id });
		}

		// U - UPDATE (Atualizar)
		public void Atualizar(Cliente cliente) {
			using var conexao = ObterConexao();
			string sql = @"
                UPDATE Clientes 
                SET Nome = @Nome, CPF = @CPF, Endereco = @Endereco, 
                    Telefone = @Telefone, Email = @Email 
                WHERE Id = @Id";

			conexao.Execute(sql, cliente);
		}

		// D - DELETE (Excluir)
		public void Excluir(int id) {
			using var conexao = ObterConexao();
			string sql = "DELETE FROM Clientes WHERE Id = @Id";

			conexao.Execute(sql, new { Id = id });
		}
	}
}