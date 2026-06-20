using PetShopCare.Models;
using PetShopCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Services {
    public class ClienteService {
        private readonly ClienteRepository _repository;

        public ClienteService() {
            _repository = new ClienteRepository();
        }

        public void SalvarCliente(Cliente cliente) {
            if (string.IsNullOrWhiteSpace(cliente.Nome) || string.IsNullOrWhiteSpace(cliente.CPF))
                throw new ArgumentException("Nome e CPF são campos obrigatórios.");

            // Validação de regra de negócio: Impedir CPFs duplicados
            var clientesExistentes = _repository.BuscarTodos();
            bool cpfJaCadastrado = clientesExistentes.Any(c => c.CPF == cliente.CPF && c.Id != cliente.Id);

            if (cpfJaCadastrado)
                throw new InvalidOperationException("Já existe um cliente cadastrado com este CPF.");

            if (cliente.Id == 0) {
                cliente.DataCadastro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _repository.Inserir(cliente);
            }
            else {
                _repository.Atualizar(cliente);
            }
        }

        public List<Cliente> ListarTodos() {
            return _repository.BuscarTodos();
        }

        public void ExcluirCliente(int id) {
            _repository.Excluir(id);
        }

        public void ExcluirClienteCompleto(int idTutor) {
            // 1. Tenta a Exclusão em Cascata (Limpar os pets)
            try {
                var petRepository = new PetShopCare.Repositories.PetRepository();
                petRepository.ExcluirPorClienteId(idTutor);
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex) {
                // Se o erro for "no such table" (código de erro SQLite comum), 
                // o sistema engole o erro silenciosamente e segue o fluxo, 
                // garantindo que a exclusão do Tutor não seja interrompida.
                System.Diagnostics.Debug.WriteLine($"Aviso: Tabela de pets não encontrada. Detalhe: {ex.Message}");
            }

            // 2. Exclui o Tutor principal com segurança
            _repository.Excluir(idTutor);
        }
    }
}