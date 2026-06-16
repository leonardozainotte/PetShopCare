using PetShopCare.Models;
using PetShopCare.Repositories;
using System;
using System.Collections.Generic;

namespace PetShopCare.Services {
    public class UsuarioService {
        private readonly UsuarioRepository _repository;

        public UsuarioService() {
            _repository = new UsuarioRepository();
        }

        public void CadastrarUsuario(Usuario usuario, string senhaAberto) {
            if (string.IsNullOrWhiteSpace(usuario.Login) || string.IsNullOrWhiteSpace(senhaAberto))
                throw new ArgumentException("Login e senha são obrigatórios.");

            var usuarioExistente = _repository.BuscarPorLogin(usuario.Login);
            if (usuarioExistente != null)
                throw new InvalidOperationException("Este login já está em uso no sistema.");

            usuario.SenhaHash = CriptografiaService.GerarHash(senhaAberto);
            usuario.DataCadastro = DateTime.Now;

            _repository.Inserir(usuario);
        }

        public Usuario Autenticar(string login, string senhaAberto) {
            var usuario = _repository.BuscarPorLogin(login);
            if (usuario == null) return null; // Usuário não encontrado

            string hashTentativa = CriptografiaService.GerarHash(senhaAberto);
            if (usuario.SenhaHash == hashTentativa && usuario.Status == "Ativo") {
                return usuario; // Autenticação bem-sucedida
            }
            return null; // Senha incorreta ou usuário inativo
        }

        public List<Usuario> ListarTodos() {
            return _repository.BuscarTodos();
        }
    }
}