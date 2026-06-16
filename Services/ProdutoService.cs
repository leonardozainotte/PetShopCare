using PetShopCare.Models;
using PetShopCare.Repositories;
using System;
using System.Collections.Generic;

namespace PetShopCare.Services {
    public class ProdutoService {
        private readonly ProdutoRepository _repository;

        public ProdutoService() {
            _repository = new ProdutoRepository();
        }

        public void SalvarProduto(Produto produto) {
            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("O nome do produto é obrigatório.");

            if (produto.PrecoVenda <= 0)
                throw new ArgumentException("O preço de venda deve ser maior que zero.");

            // Blindagem financeira: Prevenção contra prejuízo na margem de lucro
            if (produto.PrecoVenda < produto.PrecoCusto)
                throw new InvalidOperationException("Bloqueio Financeiro: O preço de venda não pode ser inferior ao preço de custo.");

            if (produto.Id == 0)
                _repository.Inserir(produto);
            else
                _repository.Atualizar(produto);
        }

        public List<Produto> ListarTodos() {
            return _repository.BuscarTodos();
        }

        public List<Produto> VerificarAlertaEstoque() {
            return _repository.BuscarProdutosAlertaEstoque();
        }
    }
}