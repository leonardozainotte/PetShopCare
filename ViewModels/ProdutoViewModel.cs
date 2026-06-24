using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;
using PetShopCare.Views;

namespace PetShopCare.ViewModels {
    public class ProdutoViewModel : ViewModelBase {
        private readonly ProdutoService _produtoService;

        private ObservableCollection<Produto> _produtos;
        public ObservableCollection<Produto> Produtos {
            get => _produtos;
            set { _produtos = value; OnPropertyChanged(); }
        }

        private string _alertaEstoqueMensagem;
        public string AlertaEstoqueMensagem {
            get => _alertaEstoqueMensagem;
            set { _alertaEstoqueMensagem = value; OnPropertyChanged(); }
        }

        // 1. Declaração das propriedades de Comando na raiz da classe
        public ICommand AbrirCadastroCommand { get; }
        public ICommand AbrirEdicaoCommand { get; }
        public ICommand LancarEstoqueCommand { get; }

        public ProdutoViewModel() {
            _produtoService = new ProdutoService();
            Produtos = new ObservableCollection<Produto>();

            // 2. Inicialização DENTRO do construtor
            AbrirCadastroCommand = new RelayCommand(ExecutarAbrirCadastro);
            AbrirEdicaoCommand = new RelayCommand(p => {
                if (p is Produto prod) {
                    var vm = new EdicaoProdutoViewModel(prod);
                    var win = new EdicaoProdutoView { DataContext = vm };
                    vm.FecharJanela = () => win.Close();

                    win.ShowDialog();

                    CarregarProdutos();
                    VerificarAlertas();
                }
            });

            LancarEstoqueCommand = new RelayCommand(p => {
                if (p is Produto prod) {
                    var vm = new LancarEstoqueViewModel(prod);
                    var win = new LancarEstoqueView { DataContext = vm };
                    vm.FecharJanela = () => win.Close();

                    win.ShowDialog();

                    // Atualiza a grade com o novo saldo e reavalia os alertas vermelhos!
                    CarregarProdutos();
                    VerificarAlertas();
                }
            });

            // 3. Cargas iniciais
            CarregarProdutos();
            VerificarAlertas();
        }

        private void ExecutarAbrirCadastro(object? p) {
            var cadastroViewModel = new CadastroProdutoViewModel();
            var janela = new CadastroProdutoView {
                DataContext = cadastroViewModel
            };

            janela.ShowDialog();

            CarregarProdutos();
            VerificarAlertas();
        }

        public void CarregarProdutos() {
            var lista = _produtoService.ListarTodos();
            Produtos.Clear();
            foreach (var p in lista) {
                Produtos.Add(p);
            }
        }

        private void VerificarAlertas() {
            var produtosEmAlerta = _produtoService.VerificarAlertaEstoque();
            if (produtosEmAlerta != null && produtosEmAlerta.Any()) {
                AlertaEstoqueMensagem = $"Atenção: {produtosEmAlerta.Count} produto(s) abaixo do estoque mínimo!";
            }
            else {
                AlertaEstoqueMensagem = string.Empty;
            }
        }
    }
}