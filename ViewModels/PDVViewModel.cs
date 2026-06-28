using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Repositories;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class PDVViewModel : ViewModelBase {
        private readonly ClienteService _clienteService;
        private readonly ProdutoService _produtoService;
        private readonly VendaRepository _vendaRepository;

        // 1. Seleção de Cliente
        public ObservableCollection<Cliente> ClientesDisponiveis { get; set; }

        private Cliente? _clienteSelecionado;
        public Cliente? ClienteSelecionado {
            get => _clienteSelecionado;
            set { _clienteSelecionado = value; OnPropertyChanged(); }
        }

        // 2. Busca de Produtos
        private string _textoPesquisaProduto = string.Empty;
        public string TextoPesquisaProduto {
            get => _textoPesquisaProduto;
            set {
                _textoPesquisaProduto = value;
                OnPropertyChanged();
                AplicarFiltroProdutos();
            }
        }

        private ObservableCollection<Produto> _produtosFiltrados;
        public ObservableCollection<Produto> ProdutosFiltrados {
            get => _produtosFiltrados;
            set { _produtosFiltrados = value; OnPropertyChanged(); }
        }
        private List<Produto> _estoqueCompleto = new List<Produto>();

        // 3. O Carrinho de Compras
        private ObservableCollection<ItemVenda> _carrinho;
        public ObservableCollection<ItemVenda> Carrinho {
            get => _carrinho;
            set { _carrinho = value; OnPropertyChanged(); }
        }

        private decimal _totalVenda;
        public decimal TotalVenda {
            get => _totalVenda;
            set { _totalVenda = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand AdicionarItemCommand { get; }
        public ICommand RemoverItemCommand { get; }
        public ICommand DiminuirQuantidadeCommand { get; }
        public ICommand FinalizarVendaCommand { get; }

        public PDVViewModel() {
            _clienteService = new ClienteService();
            _produtoService = new ProdutoService();
            _vendaRepository = new VendaRepository();

            ClientesDisponiveis = new ObservableCollection<Cliente>();
            ProdutosFiltrados = new ObservableCollection<Produto>();
            Carrinho = new ObservableCollection<ItemVenda>();

            AdicionarItemCommand = new RelayCommand(ExecutarAdicionarItem);
            RemoverItemCommand = new RelayCommand(ExecutarRemoverItem);
            DiminuirQuantidadeCommand = new RelayCommand(ExecutarDiminuirQuantidade);
            FinalizarVendaCommand = new RelayCommand(ExecutarFinalizarVenda, p => Carrinho.Count > 0);

            CarregarDadosIniciais();
        }

        private void CarregarDadosIniciais() {
            var clientes = _clienteService.ListarTodos();
            ClientesDisponiveis.Clear();
            foreach (var c in clientes) {
                ClientesDisponiveis.Add(c);
            }

            _estoqueCompleto = _produtoService.ListarTodos();
            AplicarFiltroProdutos();
        }

        private void AplicarFiltroProdutos() {
            if (string.IsNullOrWhiteSpace(TextoPesquisaProduto)) {
                ProdutosFiltrados = new ObservableCollection<Produto>(_estoqueCompleto);
                return;
            }

            var texto = TextoPesquisaProduto.ToLower();
            var filtrados = _estoqueCompleto.Where(p =>
                (p.Nome != null && p.Nome.ToLower().Contains(texto)) ||
                (p.CodigoBarras != null && p.CodigoBarras.Contains(texto))
            ).ToList();

            ProdutosFiltrados = new ObservableCollection<Produto>(filtrados);
        }

        private void ExecutarAdicionarItem(object? parameter) {
            if (parameter is Produto prod) {
                if (prod.EstoqueAtual <= 0) return;

                var itemExistente = Carrinho.FirstOrDefault(i => i.ProdutoId == prod.Id);

                if (itemExistente != null) {
                    if (itemExistente.Quantidade >= prod.EstoqueAtual) return;

                    itemExistente.Quantidade++;
                    itemExistente.Subtotal = itemExistente.Quantidade * itemExistente.PrecoUnitario;

                    var index = Carrinho.IndexOf(itemExistente);
                    Carrinho.RemoveAt(index);
                    Carrinho.Insert(index, itemExistente);
                }
                else {
                    var novoItem = new ItemVenda {
                        ProdutoId = prod.Id,
                        ProdutoNome = prod.Nome,
                        Quantidade = 1,
                        PrecoUnitario = prod.PrecoVenda,
                        Subtotal = prod.PrecoVenda
                    };
                    Carrinho.Add(novoItem);
                }

                RecalcularTotalVenda();
            }
        }

        private void ExecutarDiminuirQuantidade(object? parameter) {
            if (parameter is ItemVenda item) {
                if (item.Quantidade > 1) {
                    item.Quantidade--;
                    item.Subtotal = item.Quantidade * item.PrecoUnitario;

                    var index = Carrinho.IndexOf(item);
                    Carrinho.RemoveAt(index);
                    Carrinho.Insert(index, item);
                }
                else {
                    Carrinho.Remove(item);
                }

                RecalcularTotalVenda();
            }
        }

        private void ExecutarRemoverItem(object? parameter) {
            if (parameter is ItemVenda item) {
                Carrinho.Remove(item);
                RecalcularTotalVenda();
            }
        }

        private void RecalcularTotalVenda() {
            TotalVenda = Carrinho.Sum(i => i.Subtotal);
        }

        private void ExecutarFinalizarVenda(object? parameter) {
            try {
                // 1. Instancia o modelo principal de Venda preenchendo as propriedades necessárias
                var novaVenda = new Venda {
                    DataVenda = DateTime.Now,
                    UsuarioId = 1, // Identificador padrão do operador de caixa
                    ClienteId = ClienteSelecionado?.Id, // Passa o ID se selecionado, caso contrário envia nulo
                    ValorTotal = this.TotalVenda,
                    Desconto = 0,
                    FormaPagamento = "Dinheiro",
                    Status = "Concluida"
                };

                // 2. Converte o Carrinho para o formato List esperado pelo repositório
                var itensVenda = Carrinho.ToList();

                // 3. Executa o processamento atômico do repositório (Vendas -> ItensVenda -> Produtos -> EstoqueMovimentacao)
                _vendaRepository.ProcessarVenda(novaVenda, itensVenda);

                // 4. Limpeza do estado do caixa e feedback visual de sucesso
                System.Windows.MessageBox.Show("Venda processada com sucesso e estoque atualizado!", "PDV", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                Carrinho.Clear();
                ClienteSelecionado = null;
                TextoPesquisaProduto = string.Empty;
                RecalcularTotalVenda();

                // 5. Atualiza os dados locais para refletir os novos saldos de estoque no catálogo imediatamente
                CarregarDadosIniciais();
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show($"Erro crítico ao finalizar a transação: {ex.Message}", "Falha de Persistência", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}