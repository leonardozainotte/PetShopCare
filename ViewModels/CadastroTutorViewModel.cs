using System;
using System.Windows; // Necessário para gerir a Visibilidade do botão Excluir
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class CadastroTutorViewModel : ViewModelBase {
        private readonly ClienteService _clienteService;
        private Cliente _novoCliente;

        // Propriedades conectadas aos campos de texto da tela
        public Cliente NovoCliente {
            get => _novoCliente;
            set { _novoCliente = value; OnPropertyChanged(); }
        }

        private string _mensagemErro = string.Empty;
        public string MensagemErro {
            get => _mensagemErro;
            set { _mensagemErro = value; OnPropertyChanged(); }
        }

        // --- LÓGICA HÍBRIDA (NOVO CADASTRO VS EDIÇÃO) ---
        public bool IsModoEdicao => NovoCliente != null && NovoCliente.Id != 0;
        public string TituloJanela => IsModoEdicao ? "Editar Tutor" : "Cadastrar Novo Tutor";
        public string TextoBotaoSalvar => IsModoEdicao ? "ALTERAR" : "SALVAR";
        public Visibility VisibilidadeBotaoExcluir => IsModoEdicao ? Visibility.Visible : Visibility.Collapsed;

        // Comandos de Ação
        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand ExcluirCommand { get; }

        // Evento para avisar a Janela de que ela pode fechar
        public Action? FecharJanela { get; set; }

        // Construtor Flexível: Aceita 'null' para criar novo, ou o 'Cliente' para editar
        public CadastroTutorViewModel(Cliente clienteExistente = null) {
            _clienteService = new ClienteService();

            if (clienteExistente != null) {
                // MODO EDIÇÃO: Criamos uma cópia exata para não alterar a tabela principal em tempo real antes de salvar
                NovoCliente = new Cliente {
                    Id = clienteExistente.Id,
                    Nome = clienteExistente.Nome,
                    CPF = clienteExistente.CPF,
                    Telefone = clienteExistente.Telefone,
                    Endereco = clienteExistente.Endereco,
                    DataCadastro = clienteExistente.DataCadastro
                };
            } else {
                // MODO CADASTRO: Objeto em branco
                NovoCliente = new Cliente();
            }

            SalvarCommand = new RelayCommand(ExecutarSalvar, PodeSalvar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
            ExcluirCommand = new RelayCommand(ExecutarExcluir);
        }

        private bool PodeSalvar(object? parameter) {
            // O botão SALVAR só fica ativo se Nome, CPF e Telefone não estiverem em branco
            return !string.IsNullOrWhiteSpace(NovoCliente.Nome) &&
                   !string.IsNullOrWhiteSpace(NovoCliente.CPF) &&
                   !string.IsNullOrWhiteSpace(NovoCliente.Telefone);
        }

        private void ExecutarSalvar(object? parameter) {
            try {
                MensagemErro = string.Empty;
                // O serviço (Upsert) fará UPDATE se tiver ID, ou INSERT se o ID for 0
                _clienteService.SalvarCliente(NovoCliente);
                FecharJanela?.Invoke();
            }
            catch (InvalidOperationException ex) {
                MensagemErro = ex.Message;
            }
            catch (Exception ex) {
                MensagemErro = $"Erro do Sistema: {ex.Message}";
            }
        }

        private void ExecutarExcluir(object? parameter) {
            // Barreira de segurança antes da exclusão
            var confirmacao = MessageBox.Show(
                $"Tem certeza que deseja excluir o tutor {NovoCliente.Nome}?\nTODOS OS PETS VINCULADOS SERÃO EXCLUÍDOS TAMBÉM.",
                "Confirmação de Exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacao == MessageBoxResult.Yes) {
                try {
                    _clienteService.ExcluirClienteCompleto(NovoCliente.Id);
                    FecharJanela?.Invoke();
                }
                catch (Exception ex) {
                    MensagemErro = $"Erro ao excluir: {ex.Message}";
                }
            }
        }

        private void ExecutarCancelar(object? parameter) {
            FecharJanela?.Invoke();
        }
    }
}