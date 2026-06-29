using PetShopCare.Models;
using PetShopCare.Repositories;
using System;
using System.Collections.Generic;

namespace PetShopCare.Services {
    public class OrdemServicoService {
        private readonly OrdemServicoRepository _repository;

        public OrdemServicoService() {
            _repository = new OrdemServicoRepository();
        }

        public void Agendar(OrdemServico ordem) {
            // 1. Validações Estruturais
            if (ordem.PetId <= 0 || ordem.ClienteId <= 0 || ordem.ServicoId <= 0)
                throw new ArgumentException("É necessário selecionar o Tutor, o Pet e o Serviço.");

            // 2. Validação de Regra de Negócio (Impede agendamento retroativo)
            if (ordem.DataHoraAgendamento.Date < DateTime.Now.Date)
                throw new ArgumentException("Não é possível realizar agendamentos para datas no passado.");

            if (ordem.Valor < 0)
                throw new ArgumentException("O valor do serviço não pode ser negativo.");

            _repository.Agendar(ordem);
        }

        public List<OrdemServico> BuscarAgendamentosDoDia(DateTime data) {
            // O SQLite trabalha muito bem com datas em formato ISO (YYYY-MM-DD)
            string dataISO = data.ToString("yyyy-MM-dd");
            return _repository.BuscarAgendamentosDoDia(dataISO);
        }

        public void CancelarAgendamento(int id) {
            if (id <= 0)
                throw new ArgumentException("Identificador de agendamento inválido.");

            _repository.AtualizarStatus(id, "Cancelado");
        }
    }
}