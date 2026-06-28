using System.Windows.Input;

namespace PetShopCare.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _telaAtual;

        public object TelaAtual
        {
            get => _telaAtual;
            set
            {
                _telaAtual = value;
                OnPropertyChanged();
            }
        }

        // 1. Declaração de todas as rotas possíveis
        public ICommand AbrirDashboardCommand { get; }
        public ICommand AbrirTutoresCommand { get; }
        public ICommand AbrirProntuariosCommand { get; }
        public ICommand AbrirPdvCommand { get; }
        public ICommand AbrirProdutosCommand { get; }
        public ICommand AbrirPDVCommand { get; }

        public MainViewModel()
        {
            // 2. Mapeamento das ações (O que cada botão faz)
            AbrirDashboardCommand = new RelayCommand(p => TelaAtual = "Resumo Geral do PetShop");

            // Rota de Tutores ativa
            AbrirTutoresCommand = new RelayCommand(p => TelaAtual = new TutorViewModel());

            // Rota de Pets
            AbrirProntuariosCommand = new RelayCommand(p => TelaAtual = new PetViewModel());

            // Rota PDV
            AbrirPdvCommand = new RelayCommand(p => TelaAtual = new PDVViewModel());

            // Rota de Produtos Corrigida e Padronizada
            AbrirProdutosCommand = new RelayCommand(p => TelaAtual = new ProdutoViewModel());

            // 3. Define a tela inicial obrigatória
            TelaAtual = "Resumo Geral do PetShop";
        }
    }
}