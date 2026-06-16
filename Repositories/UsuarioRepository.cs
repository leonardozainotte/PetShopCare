using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories {
    public class UsuarioRepository {
        private SqliteConnection ObterConexao() {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        // C - CREATE
        public void Inserir(Usuario usuario) {
            using var conexao = ObterConexao();
            // Mapeando a propriedade 'Login' para a coluna 'Usuario' do banco
            string sql = @"
                INSERT INTO Usuarios (Nome, Usuario, SenhaHash, Cargo, Status) 
                VALUES (@Nome, @Login, @SenhaHash, @Cargo, @Status)";

            conexao.Execute(sql, usuario);
        }

        // R - READ (Todos)
        public List<Usuario> BuscarTodos() {
            using var conexao = ObterConexao();
            // 'Usuario AS Login' garante a hidratação correta do objeto C#
            string sql = "SELECT Id, Nome, Usuario AS Login, SenhaHash, Cargo, Status, DataCadastro FROM Usuarios ORDER BY Nome";

            return conexao.Query<Usuario>(sql).ToList();
        }

        // R - READ (Por ID)
        public Usuario BuscarPorId(int id) {
            using var conexao = ObterConexao();
            string sql = "SELECT Id, Nome, Usuario AS Login, SenhaHash, Cargo, Status, DataCadastro FROM Usuarios WHERE Id = @Id";

            return conexao.QueryFirstOrDefault<Usuario>(sql, new { Id = id });
        }

        // R - READ ESTRATÉGICO (Buscar por Login - Usado na Autenticação)
        public Usuario BuscarPorLogin(string login) {
            using var conexao = ObterConexao();
            string sql = "SELECT Id, Nome, Usuario AS Login, SenhaHash, Cargo, Status, DataCadastro FROM Usuarios WHERE Usuario = @Login";

            return conexao.QueryFirstOrDefault<Usuario>(sql, new { Login = login });
        }

        // U - UPDATE
        public void Atualizar(Usuario usuario) {
            using var conexao = ObterConexao();
            string sql = @"
                UPDATE Usuarios 
                SET Nome = @Nome, Usuario = @Login, SenhaHash = @SenhaHash, 
                    Cargo = @Cargo, Status = @Status 
                WHERE Id = @Id";

            conexao.Execute(sql, usuario);
        }

        // D - DELETE
        public void Excluir(int id) {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Usuarios WHERE Id = @Id";

            conexao.Execute(sql, new { Id = id });
        }
    }
}