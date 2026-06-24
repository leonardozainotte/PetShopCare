using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class CadastroPetViewModel : ViewModelBase {
        private readonly PetService _petService;
        private readonly ClienteService _clienteService;

        public Pet NovoPet { get; set; }
        public ObservableCollection<Cliente> TutoresDisponiveis { get; set; }

        // Novas listas para suportar a seleção inteligente
        public ObservableCollection<string> EspeciesDisponiveis { get; set; }
        public ObservableCollection<string> RacasDisponiveis { get; set; }
        public ObservableCollection<string> SexosDisponiveis { get; set; }

        private string _especieSelecionada = string.Empty;
        public string EspecieSelecionada {
            get => _especieSelecionada;
            set {
                _especieSelecionada = value;
                if (NovoPet != null) NovoPet.Especie = value;
                OnPropertyChanged();

                // Gatilho: Quando a espécie muda, atualizamos a lista de raças disponíveis
                AtualizarRacasDisponiveis();
            }
        }

        private string _mensagemErro = string.Empty;
        public string MensagemErro {
            get => _mensagemErro;
            set { _mensagemErro = value; OnPropertyChanged(); }
        }

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }
        public Action? FecharJanela { get; set; }

        public CadastroPetViewModel() {
            _petService = new PetService();
            _clienteService = new ClienteService();

            NovoPet = new Pet();
            TutoresDisponiveis = new ObservableCollection<Cliente>();

            // Inicializa as novas coleções
            EspeciesDisponiveis = new ObservableCollection<string> { "Cão", "Gato", "Outros" };
            RacasDisponiveis = new ObservableCollection<string>();
            SexosDisponiveis = new ObservableCollection<string> { "Macho", "Fêmea", "Não Identificado" };

            CarregarTutores();

            SalvarCommand = new RelayCommand(ExecutarSalvar, PodeSalvar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
        }

        private void CarregarTutores() {
            try {
                var tutores = _clienteService.ListarTodos();
                if (tutores != null) {
                    foreach (var tutor in tutores) {
                        TutoresDisponiveis.Add(tutor);
                    }
                }
            }
            catch (Exception) {
                MensagemErro = "Erro ao carregar lista de tutores.";
            }
        }

        private string _pesoTexto = string.Empty;
        public string PesoTexto {
            get => _pesoTexto;
            set {
                _pesoTexto = value;

                // Converte ponto para vírgula garantindo o padrão de digitação
                string valorLimpo = value?.Replace('.', ',') ?? "0";

                if (decimal.TryParse(valorLimpo, out decimal pesoConvertido)) {
                    if (NovoPet != null) NovoPet.Peso = pesoConvertido;
                }
                else {
                    if (NovoPet != null) NovoPet.Peso = null; // Define como nulo se o campo for limpo ou inválido
                }

                OnPropertyChanged();
            }
        }

        // Alimenta a lista de raças de acordo com a espécie escolhida
        private void AtualizarRacasDisponiveis() {
            RacasDisponiveis.Clear();

            if (EspecieSelecionada == "Cão") {
                RacasDisponiveis.Add("Vira-lata (SRD)");
                RacasDisponiveis.Add("Poodle");
                RacasDisponiveis.Add("Labrador Retriever");
                RacasDisponiveis.Add("Golden Retriever");
                RacasDisponiveis.Add("Bulldog Francês");
                RacasDisponiveis.Add("Pinscher");
                RacasDisponiveis.Add("Shih Tzu");
                RacasDisponiveis.Add("Pastor Alemão");
            }
            else if (EspecieSelecionada == "Gato") {
                RacasDisponiveis.Add("Vira-lata (SRD)");
                RacasDisponiveis.Add("Persa");
                RacasDisponiveis.Add("Siamês");
                RacasDisponiveis.Add("Maine Coon");
                RacasDisponiveis.Add("Angorá");
                RacasDisponiveis.Add("Sphynx");
            }
            else if (EspecieSelecionada == "Outros") {
                RacasDisponiveis.Add("Calopsita");
                RacasDisponiveis.Add("Coelho");
                RacasDisponiveis.Add("Hamster");
                RacasDisponiveis.Add("Porquinho da Índia");
            }

            // Limpa o campo da raça do modelo para evitar inconsistências (ex: selecionar Cão e a raça continuar Siamês)
            if (NovoPet != null) NovoPet.Raca = string.Empty;

            if (NovoPet != null) NovoPet.Cor = string.Empty;
        }

        private bool PodeSalvar(object? parameter) {
            return !string.IsNullOrWhiteSpace(NovoPet.Nome) &&
                   !string.IsNullOrWhiteSpace(NovoPet.Especie) &&
                   NovoPet.ClienteId > 0;
        }

        private void ExecutarSalvar(object? parameter) {
            try {
                MensagemErro = string.Empty;
                _petService.SalvarPet(NovoPet);
                FecharJanela?.Invoke();
            }
            catch (Exception ex) {
                MensagemErro = $"Erro: {ex.Message}";
            }
        }

        private void ExecutarCancelar(object? parameter) => FecharJanela?.Invoke();
    }
}