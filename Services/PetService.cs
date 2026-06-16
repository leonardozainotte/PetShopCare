using PetShopCare.Models;
using PetShopCare.Repositories;
using System;
using System.Collections.Generic;

namespace PetShopCare.Services {
    public class PetService {
        private readonly PetRepository _repository;

        public PetService() {
            _repository = new PetRepository();
        }

        public void SalvarPet(Pet pet) {
            if (string.IsNullOrWhiteSpace(pet.Nome))
                throw new ArgumentException("O nome do pet é obrigatório.");

            // Validação relacional: Um pet não pode existir "órfão" no sistema
            if (pet.ClienteId <= 0)
                throw new ArgumentException("O pet deve estar vinculado a um tutor (cliente) válido.");

            if (pet.Id == 0)
                _repository.Inserir(pet);
            else
                _repository.Atualizar(pet);
        }

        public List<Pet> ListarPorCliente(int clienteId) {
            return _repository.BuscarPorClienteId(clienteId);
        }

        public void ExcluirPet(int id) {
            _repository.Excluir(id);
        }
    }
}