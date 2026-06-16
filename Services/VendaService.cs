using PetShopCare.Models;
using PetShopCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetShopCare.Services {
    public class VendaService {
        private readonly VendaRepository _vendaRepository;
        private readonly ProdutoRepository _produtoRepository;

        public VendaService() {
            _vendaRepository = new VendaRepository();
            _produtoRepository = new ProdutoRepository();
        }

        public void RealizarVenda(Venda venda, List<ItemVenda> itens) {
            if (itens == null || !itens.Any())
                throw new ArgumentException("A venda deve conter pelo menos um item no carrinho.");

            if (venda.UsuarioId <= 0)
                throw new ArgumentException("Obrigatoriedade de auditoria: A venda precisa registrar o operador responsável.");

            // Validação de Regra de Negócio: Impede estoque negativo antes de abrir a transação de banco
            foreach (var item in itens) {
                var produto = _produtoRepository.BuscarPorId(item.ProdutoId);

                if (produto == null)
                    throw new InvalidOperationException($"Produto ID {item.ProdutoId} não reconhecido pelo sistema.");

                if (produto.EstoqueAtual < item.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente para '{produto.Nome}'. Disponível: {produto.EstoqueAtual} | Solicitado: {item.Quantidade}.");
            }

            // O carimbo de tempo é gerado pela máquina no exato momento em que a venda passa nas validações
            venda.DataVenda = DateTime.Now;

            // Repassa para o repositório que realizará a inserção atômica (Venda + Itens + Baixa de Estoque)
            _vendaRepository.ProcessarVenda(venda, itens);
        }
    }
}