using PetShopCare.Models;
using PetShopCare.Repositories;
using System;
using System.Collections.Generic;

namespace PetShopCare.Services {
    public class ServicoService {
        private readonly ServicoRepository _repository;

        public ServicoService() {
            _repository = new ServicoRepository();
        }

        public List<Servico> BuscarTodos() {
            return _repository.BuscarTodos();
        }

        public void Adicionar(Servico servico) {
            if (string.IsNullOrWhiteSpace(servico.Nome))
                throw new ArgumentException("O nome do serviço é obrigatório.");

            if (servico.Preco < 0)
                throw new ArgumentException("O preço não pode ser negativo.");

            if (servico.TempoEstimadoMinutos <= 0)
                throw new ArgumentException("O tempo estimado deve ser maior que zero.");

            _repository.Adicionar(servico);
        }

        public void Atualizar(Servico servico) {
            if (string.IsNullOrWhiteSpace(servico.Nome))
                throw new ArgumentException("O nome do serviço é obrigatório.");

            if (servico.Preco < 0)
                throw new ArgumentException("O preço não pode ser negativo.");

            if (servico.TempoEstimadoMinutos <= 0)
                throw new ArgumentException("O tempo estimado deve ser maior que zero.");

            _repository.Atualizar(servico);
        }

        public void Excluir(int id) {
            _repository.Excluir(id);
        }
    }
}