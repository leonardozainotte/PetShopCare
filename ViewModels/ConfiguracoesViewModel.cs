using PetShopCare.Models;
using PetShopCare.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class ConfiguracoesViewModel : ViewModelBase {
        private readonly UsuarioRepository _repository;
        public ObservableCollection<Usuario> ListaUsuarios { get; set; }

        // Variável oculta que diz ao sistema se estamos a Criar (0) ou a Editar (> 0)
        private int _idAtual = 0;

        // =========================================================
        // CAMPOS DO FORMULÁRIO 
        // =========================================================
        private string _nome;
        public string Nome { get => _nome; set { _nome = value; OnPropertyChanged(); } }

        private string _login;
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }

        private string _senhaInicial;
        public string SenhaInicial { get => _senhaInicial; set { _senhaInicial = value; OnPropertyChanged(); } }

        private int _perfilSelecionado = 3;
        public int PerfilSelecionado { get => _perfilSelecionado; set { _perfilSelecionado = value; OnPropertyChanged(); } }

        // O texto do botão muda automaticamente
        public string TextoBotaoSalvar => _idAtual == 0 ? "Cadastrar" : "Salvar Alterações";

        // Deteta quando o administrador clica numa linha da tabela
        private Usuario _usuarioSelecionado;
        public Usuario UsuarioSelecionado {
            get => _usuarioSelecionado;
            set {
                _usuarioSelecionado = value;
                OnPropertyChanged();
                if (_usuarioSelecionado != null) PrepararEdicao(_usuarioSelecionado);
            }
        }

        // =========================================================
        // COMANDOS
        // =========================================================
        public ICommand SalvarUsuarioCommand { get; }
        public ICommand LimparFormularioCommand { get; } // Novo botão para cancelar edição

        public ConfiguracoesViewModel() {
            _repository = new UsuarioRepository();
            ListaUsuarios = new ObservableCollection<Usuario>();

            CarregarUsuarios();

            SalvarUsuarioCommand = new RelayCommand(p => SalvarUsuario(), p => PodeSalvar());
            LimparFormularioCommand = new RelayCommand(p => LimparCampos(), p => PodeLimpar());
        }

        private void CarregarUsuarios() {
            ListaUsuarios.Clear();
            var usuarios = _repository.ObterTodos();
            foreach (var user in usuarios) {
                ListaUsuarios.Add(user);
            }
        }

        // Puxa os dados da tabela para os campos de texto
        private void PrepararEdicao(Usuario usuario) {
            _idAtual = usuario.Id;
            Nome = usuario.Nome;
            Login = usuario.Login;
            SenhaInicial = string.Empty; // Deixa vazio para não forçar a troca de senha
            PerfilSelecionado = (int)usuario.Perfil;
            OnPropertyChanged(nameof(TextoBotaoSalvar)); // Atualiza o texto do botão
        }

        private void LimparCampos() {
            _idAtual = 0;
            Nome = string.Empty;
            Login = string.Empty;
            SenhaInicial = string.Empty;
            PerfilSelecionado = 3;
            UsuarioSelecionado = null;
            OnPropertyChanged(nameof(TextoBotaoSalvar)); // Volta o texto para "Cadastrar"
        }

        private bool PodeLimpar() {
            return _idAtual > 0 ||
                   !string.IsNullOrWhiteSpace(Nome) ||
                   !string.IsNullOrWhiteSpace(Login) ||
                   !string.IsNullOrWhiteSpace(SenhaInicial);
        }

        private bool PodeSalvar() {
            // Se for Novo (_idAtual == 0), a senha é obrigatória.
            // Se for Edição (_idAtual > 0), a senha é opcional.
            if (_idAtual == 0)
                return !string.IsNullOrWhiteSpace(Nome) && !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(SenhaInicial);
            else
                return !string.IsNullOrWhiteSpace(Nome) && !string.IsNullOrWhiteSpace(Login);
        }

        private void SalvarUsuario() {
            try {
                bool atualizarSenha = !string.IsNullOrWhiteSpace(SenhaInicial);
                string hash = atualizarSenha ? CriptografiaService.GerarHash(SenhaInicial) : null;

                var usuario = new Usuario {
                    Id = _idAtual, // Crucial para o comando UPDATE saber quem alterar
                    Nome = this.Nome,
                    Login = this.Login,
                    SenhaHash = hash,
                    Perfil = (PerfilAcesso)this.PerfilSelecionado,
                    Cargo = this.PerfilSelecionado == 1 ? "Gerente" : (this.PerfilSelecionado == 2 ? "Atendente" : "Operador")
                };

                if (_idAtual == 0) {
                    _repository.Adicionar(usuario);
                    MessageBox.Show("Funcionário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    _repository.Atualizar(usuario, atualizarSenha);
                    MessageBox.Show("Dados atualizados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LimparCampos();
                CarregarUsuarios();
            }
            catch (System.Exception) {
                MessageBox.Show($"Erro ao gravar: O utilizador '{Login}' provavelmente já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}