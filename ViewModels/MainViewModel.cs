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
        public ICommand AbrirProdutosCommand { get; }
        public ICommand AbrirPdvCommand { get; }
        public ICommand AbrirAgendamentoCommand { get; }
        public ICommand AbrirServicosCommand { get; }

        public MainViewModel()
        {
            // 2. Mapeamento das ações (O que cada botão faz)
            AbrirDashboardCommand = new RelayCommand(p => TelaAtual = new DashboardViewModel());

            // Rotas de Cadastros e Catálogo
            AbrirTutoresCommand = new RelayCommand(p => TelaAtual = new TutorViewModel());
            AbrirProntuariosCommand = new RelayCommand(p => TelaAtual = new PetViewModel());
            AbrirProdutosCommand = new RelayCommand(p => TelaAtual = new ProdutoViewModel());
            AbrirServicosCommand = new RelayCommand(p => TelaAtual = new ServicoViewModel());

            // Rotas Operacionais
            AbrirPdvCommand = new RelayCommand(p => TelaAtual = new PDVViewModel());

            // Rota de Agendamento alinhada ao seu padrão!
            AbrirAgendamentoCommand = new RelayCommand(p => TelaAtual = new AgendamentoViewModel());

            // 3. Define a tela inicial obrigatória
            TelaAtual = new DashboardViewModel();
        }
    }
}