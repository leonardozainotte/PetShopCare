using Dapper;
using Microsoft.Data.Sqlite;
using PetShopCare.Database;
using PetShopCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Repositories
{
    public class UsuarioRepository
    {
        private SqliteConnection ObterConexao()
        {
            return new SqliteConnection(DatabaseConfig.ConnectionString);
        }

        // C - CREATE
        public void Inserir(Usuario usuario)
        {
            using var conexao = ObterConexao();
            string sql = @"
                INSERT INTO Usuarios (Nome, Login, SenhaHash, Cargo, Status, DataCadastro, Perfil) 
                VALUES (@Nome, @Login, @SenhaHash, @Cargo, @Status, @DataCadastro, @Perfil)";
            conexao.Execute(sql, usuario);
        }

        // R - READ (Todos)
        public List<Usuario> BuscarTodos()
        {
            using var conexao = ObterConexao();
            // 'Usuario AS Login' garante a hidratação correta do objeto C#
            string sql = "SELECT Id, Nome, Usuario AS Login, SenhaHash, Cargo, Status, DataCadastro, Perfil FROM Usuarios ORDER BY Nome";

            return conexao.Query<Usuario>(sql).ToList();
        }

        // R - READ (Por ID)
        public Usuario BuscarPorId(int id)
        {
            using var conexao = ObterConexao();
            string sql = "SELECT Id, Nome, Usuario AS Login, SenhaHash, Cargo, Status, DataCadastro, Perfil FROM Usuarios WHERE Id = @Id";

            return conexao.QueryFirstOrDefault<Usuario>(sql, new { Id = id });
        }

        // R - READ ESTRATÉGICO (Buscar por Login - Usado na Autenticação)
        public Usuario BuscarPorLogin(string login)
        {
            using var conexao = ObterConexao();
            string sql = "SELECT Id, Nome, Usuario AS Login, SenhaHash, Cargo, Status, DataCadastro, Perfil FROM Usuarios WHERE Usuario = @Login";

            return conexao.QueryFirstOrDefault<Usuario>(sql, new { Login = login });
        }

        // U - UPDATE
        public void Atualizar(Usuario usuario, bool atualizarSenha)
        {
            using var conexao = new Microsoft.Data.Sqlite.SqliteConnection(Database.DatabaseConfig.ConnectionString);
            string sql;

            if (atualizarSenha)
            {
                // Atualiza todos os dados, incluindo uma nova senha
                sql = @"
                    UPDATE Usuarios 
                    SET Nome = @Nome, Usuario = @Login, SenhaHash = @SenhaHash, Cargo = @Cargo, Perfil = @Perfil 
                    WHERE Id = @Id";
            }
            else
            {
                // Atualiza apenas os dados, preservando a senha antiga intacta
                sql = @"
                    UPDATE Usuarios 
                    SET Nome = @Nome, Usuario = @Login, Cargo = @Cargo, Perfil = @Perfil 
                    WHERE Id = @Id";
            }

            Dapper.SqlMapper.Execute(conexao, sql, usuario);
        }

        // D - DELETE
        public void Excluir(int id)
        {
            using var conexao = ObterConexao();
            string sql = "DELETE FROM Usuarios WHERE Id = @Id";

            conexao.Execute(sql, new { Id = id });
        }

        public System.Collections.Generic.List<Usuario> ObterTodos()
        {
            using var conexao = new Microsoft.Data.Sqlite.SqliteConnection(Database.DatabaseConfig.ConnectionString);
            string sql = "SELECT Id, Nome, Usuario AS Login, Cargo, Status, Perfil FROM Usuarios ORDER BY Nome";
            return Dapper.SqlMapper.Query<Usuario>(conexao, sql).AsList();
        }

        // C - CREATE (Cadastrar novo funcionário)
        public void Adicionar(Usuario usuario)
        {
            using var conexao = new Microsoft.Data.Sqlite.SqliteConnection(Database.DatabaseConfig.ConnectionString);
            string sql = @"
                INSERT INTO Usuarios (Nome, Usuario, SenhaHash, Cargo, Status, Perfil) 
                VALUES (@Nome, @Login, @SenhaHash, @Cargo, 'Ativo', @Perfil)";

            Dapper.SqlMapper.Execute(conexao, sql, usuario);
        }
    }
}