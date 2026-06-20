using System;
using System.Windows;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class EdicaoTutorViewModel : ViewModelBase {
        private readonly ClienteService _clienteService;
        private Cliente _clienteEditado;

        public Cliente ClienteEditado {
            get => _clienteEditado;
            set { _clienteEditado = value; OnPropertyChanged(); }
        }

        private string _mensagemErro = string.Empty;
        public string MensagemErro {
            get => _mensagemErro;
            set { _mensagemErro = value; OnPropertyChanged(); }
        }

        public ICommand AlterarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public Action? FecharJanela { get; set; }

        public EdicaoTutorViewModel(Cliente clienteExistente) {
            _clienteService = new ClienteService();

            // Clone do objeto para evitar edições "em tempo real" na tabela por trás
            ClienteEditado = new Cliente {
                Id = clienteExistente.Id,
                Nome = clienteExistente.Nome,
                CPF = clienteExistente.CPF,
                Telefone = clienteExistente.Telefone,
                Endereco = clienteExistente.Endereco,
                DataCadastro = clienteExistente.DataCadastro
            };

            AlterarCommand = new RelayCommand(ExecutarAlterar, PodeAlterar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
            ExcluirCommand = new RelayCommand(ExecutarExcluir);
        }

        private bool PodeAlterar(object? parameter) {
            return !string.IsNullOrWhiteSpace(ClienteEditado.Nome) &&
                   !string.IsNullOrWhiteSpace(ClienteEditado.CPF) &&
                   !string.IsNullOrWhiteSpace(ClienteEditado.Telefone);
        }

        private void ExecutarAlterar(object? parameter) {
            try {
                MensagemErro = string.Empty;
                _clienteService.SalvarCliente(ClienteEditado);
                FecharJanela?.Invoke();
            }
            catch (Exception ex) {
                MensagemErro = $"Erro: {ex.Message}";
            }
        }

        private void ExecutarExcluir(object? parameter) {
            var confirmacao = MessageBox.Show(
                $"Deseja realmente excluir o tutor {ClienteEditado.Nome}?\nTODOS OS PETS VINCULADOS SERÃO EXCLUÍDOS.",
                "Aviso Crítico", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirmacao == MessageBoxResult.Yes) {
                try {
                    _clienteService.ExcluirClienteCompleto(ClienteEditado.Id);
                    FecharJanela?.Invoke();
                }
                catch (Exception ex) {
                    MensagemErro = $"Erro ao excluir: {ex.Message}";
                }
            }
        }

        private void ExecutarCancelar(object? parameter) => FecharJanela?.Invoke();
    }
}