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

        public MainViewModel()
        {
            // 2. Mapeamento das ações (O que cada botão faz)
            AbrirDashboardCommand = new RelayCommand(p => TelaAtual = "Resumo Geral do PetShop");
            AbrirTutoresCommand = new RelayCommand(p => TelaAtual = new TutorViewModel());
            AbrirProntuariosCommand = new RelayCommand(p => TelaAtual = "Módulo de Prontuários (Em Desenvolvimento...)");
            AbrirPdvCommand = new RelayCommand(p => TelaAtual = "Ponto de Venda (Em Desenvolvimento...)");

            // 3. Define a tela inicial obrigatória
            TelaAtual = "Resumo Geral do PetShop";
        }
    }
}