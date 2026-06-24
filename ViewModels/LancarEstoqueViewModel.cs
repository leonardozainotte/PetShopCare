using System;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Repositories;

namespace PetShopCare.ViewModels {
    public class LancarEstoqueViewModel : ViewModelBase {
        private readonly EstoqueRepository _repository;
        public Produto ProdutoAlvo { get; }
        public EstoqueMovimentacao Movimentacao { get; set; }

        public ICommand ConfirmarCommand { get; }
        public ICommand CancelarCommand { get; }
        public Action? FecharJanela { get; set; }

        public LancarEstoqueViewModel(Produto produto) {
            _repository = new EstoqueRepository();
            ProdutoAlvo = produto;

            Movimentacao = new EstoqueMovimentacao {
                ProdutoId = produto.Id,
                DataMovimentacao = DateTime.Now,
                TipoMovimentacao = "Entrada", // Padrão
                UsuarioId = 1 // Simulando usuário logado
            };

            ConfirmarCommand = new RelayCommand(p => {
                if (Movimentacao.Quantidade <= 0) return;
                _repository.RegistrarMovimentacao(Movimentacao);
                FecharJanela?.Invoke();
            });

            CancelarCommand = new RelayCommand(p => FecharJanela?.Invoke());
        }
    }
}