using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class EdicaoPetViewModel : ViewModelBase {
        private readonly PetService _petService;
        private readonly ClienteService _clienteService;

        public Pet PetEditado { get; set; }
        public ObservableCollection<Cliente> TutoresDisponiveis { get; set; }
        public ObservableCollection<string> EspeciesDisponiveis { get; set; }
        public ObservableCollection<string> RacasDisponiveis { get; set; }
        public ObservableCollection<string> SexosDisponiveis { get; set; }

        private string _especieSelecionada = string.Empty;
        public string EspecieSelecionada {
            get => _especieSelecionada;
            set {
                _especieSelecionada = value;
                if (PetEditado != null) PetEditado.Especie = value;
                OnPropertyChanged();
                AtualizarRacasDisponiveis();
            }
        }

        private string _pesoTexto = string.Empty;
        public string PesoTexto {
            get => _pesoTexto;
            set {
                _pesoTexto = value;
                string valorLimpo = value?.Replace('.', ',') ?? "0";
                if (decimal.TryParse(valorLimpo, out decimal pesoConvertido)) {
                    if (PetEditado != null) PetEditado.Peso = pesoConvertido;
                }
                else {
                    if (PetEditado != null) PetEditado.Peso = null;
                }
                OnPropertyChanged();
            }
        }

        private string _mensagemErro = string.Empty;
        public string MensagemErro {
            get => _mensagemErro;
            set { _mensagemErro = value; OnPropertyChanged(); }
        }

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public Action? FecharJanela { get; set; }

        public EdicaoPetViewModel(Pet petOriginal) {
            _petService = new PetService();
            _clienteService = new ClienteService();

            // Clonamos o objeto para proteger a integridade visual da DataGrid
            PetEditado = new Pet {
                Id = petOriginal.Id,
                ClienteId = petOriginal.ClienteId,
                Nome = petOriginal.Nome,
                Especie = petOriginal.Especie,
                Raca = petOriginal.Raca,
                Sexo = petOriginal.Sexo,
                DataNascimento = petOriginal.DataNascimento,
                Peso = petOriginal.Peso,
                Observacoes = petOriginal.Observacoes
            };

            TutoresDisponiveis = new ObservableCollection<Cliente>();
            EspeciesDisponiveis = new ObservableCollection<string> { "Cão", "Gato", "Outros" };
            RacasDisponiveis = new ObservableCollection<string>();
            SexosDisponiveis = new ObservableCollection<string> { "Macho", "Fêmea", "Não Identificado" };

            CarregarTutores();

            // Carregamento estratégico: Especie deve ser setada antes da Raça
            EspecieSelecionada = petOriginal.Especie ?? string.Empty;
            PetEditado.Raca = petOriginal.Raca;
            PesoTexto = petOriginal.Peso?.ToString() ?? string.Empty;

            SalvarCommand = new RelayCommand(ExecutarSalvar, PodeSalvar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
            ExcluirCommand = new RelayCommand(ExecutarExcluir);
        }

        private void CarregarTutores() {
            try {
                var tutores = _clienteService.ListarTodos();
                if (tutores != null) {
                    foreach (var tutor in tutores) TutoresDisponiveis.Add(tutor);
                }
            }
            catch { MensagemErro = "Erro ao carregar tutores."; }
        }

        private void AtualizarRacasDisponiveis() {
            RacasDisponiveis.Clear();
            if (EspecieSelecionada == "Cão") {
                string[] r = { "Vira-lata (SRD)", "Poodle", "Labrador", "Golden Retriever", "Bulldog", "Pinscher", "Shih Tzu", "Pastor Alemão" };
                foreach (var item in r) RacasDisponiveis.Add(item);
            }
            else if (EspecieSelecionada == "Gato") {
                string[] r = { "Vira-lata (SRD)", "Persa", "Siamês", "Maine Coon", "Angorá" };
                foreach (var item in r) RacasDisponiveis.Add(item);
            }
            if (PetEditado != null) PetEditado.Raca = string.Empty;
        }

        private void ExecutarExcluir(object? p) {
            // Exibe um aviso de segurança antes de apagar do banco
            var resultado = System.Windows.MessageBox.Show(
                $"Tem certeza que deseja excluir o prontuário do(a) pet {PetEditado.Nome}?\nEsta ação não poderá ser desfeita.",
                "Confirmar Exclusão",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (resultado == System.Windows.MessageBoxResult.Yes) {
                try {
                    _petService.ExcluirPet(PetEditado.Id);
                    FecharJanela?.Invoke(); // Fecha a janela e atualiza a grelha
                }
                catch (Exception ex) {
                    MensagemErro = $"Erro ao excluir: {ex.Message}";
                }
            }
        }

        private bool PodeSalvar(object? p) => !string.IsNullOrWhiteSpace(PetEditado.Nome) && !string.IsNullOrWhiteSpace(PetEditado.Especie) && PetEditado.ClienteId > 0;

        private void ExecutarSalvar(object? p) {
            try {
                _petService.SalvarPet(PetEditado);
                FecharJanela?.Invoke();
            }
            catch (Exception ex) { MensagemErro = $"Erro: {ex.Message}"; }
        }

        private void ExecutarCancelar(object? p) => FecharJanela?.Invoke();
    }
}