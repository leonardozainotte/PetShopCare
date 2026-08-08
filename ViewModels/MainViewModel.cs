using System.Windows;
using System.Windows.Input;
using PetShopCare.Models;

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

        public Usuario UsuarioLogado { get; private set; }

        // =========================================================================
        // PROPRIEDADES DE CONTROLE DE ACESSO VISUAL (RBAC)
        // =========================================================================

        public string SaudacaoUsuario => $"Olá, {UsuarioLogado.Nome}";
        public bool IsLogoutRequested { get; private set; } = false;

        // Fazemos o cast (int) para comparar o Enumerador com o número do banco de dados
        public Visibility VisibilidadeDashboard =>
            (int)UsuarioLogado.Perfil == 1 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility VisibilidadeEstoque =>
            (int)UsuarioLogado.Perfil == 1 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility VisibilidadePDV =>
            ((int)UsuarioLogado.Perfil == 1 || (int)UsuarioLogado.Perfil == 2)
            ? Visibility.Visible : Visibility.Collapsed;

        public Visibility VisibilidadeConfiguracoes =>
            (int)UsuarioLogado.Perfil == 1 ? Visibility.Visible : Visibility.Collapsed;

        public ICommand AbrirDashboardCommand { get; }
        public ICommand AbrirTutoresCommand { get; }
        public ICommand AbrirProntuariosCommand { get; }
        public ICommand AbrirProdutosCommand { get; }
        public ICommand AbrirPdvCommand { get; }
        public ICommand AbrirAgendamentoCommand { get; }
        public ICommand AbrirServicosCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand AbrirConfiguracoesCommand { get; }

        public MainViewModel(Usuario usuario)
        {
            UsuarioLogado = usuario;

            AbrirDashboardCommand = new RelayCommand(p => TelaAtual = new DashboardViewModel());
            AbrirTutoresCommand = new RelayCommand(p => TelaAtual = new TutorViewModel());
            AbrirProntuariosCommand = new RelayCommand(p => TelaAtual = new PetViewModel());
            AbrirProdutosCommand = new RelayCommand(p => TelaAtual = new ProdutoViewModel());
            AbrirServicosCommand = new RelayCommand(p => TelaAtual = new ServicoViewModel());
            AbrirPdvCommand = new RelayCommand(p => TelaAtual = new PDVViewModel());
            AbrirAgendamentoCommand = new RelayCommand(p => TelaAtual = new AgendamentoViewModel());
            LogoutCommand = new RelayCommand(p => RealizarLogout());
            AbrirConfiguracoesCommand = new RelayCommand(p => TelaAtual = new ConfiguracoesViewModel());

            // Redirecionamento inicial baseado no Perfil
            if ((int)UsuarioLogado.Perfil == 1)
            {
                TelaAtual = new DashboardViewModel();
            }
            else
            {
                TelaAtual = new TutorViewModel();
            }
        }

        private void RealizarLogout()
        {
            // Avisa o maestro (App.xaml.cs) que o fechamento foi proposital pelo botão
            IsLogoutRequested = true;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is Views.MainView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}