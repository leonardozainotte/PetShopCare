using PetShopCare.Models;
using PetShopCare.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace PetShopCare.ViewModels
{
    public class CadastroServicoViewModel : ViewModelBase
    {
        private readonly ServicoService _servicoService;
        private readonly ServicoViewModel _parentViewModel; // Para atualizar a tabela principal
        private Servico _servicoAtual;
        private string _tituloAcao;

        public Servico ServicoAtual
        {
            get => _servicoAtual;
            set
            {
                _servicoAtual = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // Força a validação do botão Salvar
            }
        }

        public string TituloAcao
        {
            get => _tituloAcao;
            set { _tituloAcao = value; OnPropertyChanged(); }
        }

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }
		public Action? FecharJanela { get; set; }

		// Construtor para NOVO Serviço
		public CadastroServicoViewModel(ServicoViewModel parentViewModel)
        {
            _servicoService = new ServicoService();
            _parentViewModel = parentViewModel;
            
            TituloAcao = "Novo Serviço";
            ServicoAtual = new Servico(); // Instância limpa

            SalvarCommand = new RelayCommand(ExecutarSalvar, PodeSalvar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
        }

        // Construtor para EDITAR Serviço (Sobrecarga)
        public CadastroServicoViewModel(ServicoViewModel parentViewModel, Servico servicoParaEditar)
        {
            _servicoService = new ServicoService();
            _parentViewModel = parentViewModel;
            
            TituloAcao = "Editar Serviço";
            
            // Clonagem (Safe Editing): Desacopla da DataGrid para evitar atualizações fantasmas
            ServicoAtual = new Servico
            {
                Id = servicoParaEditar.Id,
                Nome = servicoParaEditar.Nome,
                Preco = servicoParaEditar.Preco,
                TempoEstimadoMinutos = servicoParaEditar.TempoEstimadoMinutos
            };

            SalvarCommand = new RelayCommand(ExecutarSalvar, PodeSalvar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
        }

        private bool PodeSalvar(object obj)
        {
            return ServicoAtual != null &&
                   !string.IsNullOrWhiteSpace(ServicoAtual.Nome) &&
                   ServicoAtual.Preco > 0 &&
                   ServicoAtual.TempoEstimadoMinutos > 0;
        }

		private void ExecutarSalvar(object obj) {
			try {
				if (ServicoAtual.Id == 0) {
					_servicoService.Adicionar(ServicoAtual);
					MessageBox.Show("Serviço cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else {
					_servicoService.Atualizar(ServicoAtual);
					MessageBox.Show("Serviço atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				_parentViewModel.CarregarServicos();

				// Dispara a ação de fechar injetada pela View
				FecharJanela?.Invoke();
			}
			catch (Exception ex) {
				MessageBox.Show($"Erro ao salvar o serviço: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ExecutarCancelar(object obj) {
			FecharJanela?.Invoke();
		}
	}
}