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
    }
}