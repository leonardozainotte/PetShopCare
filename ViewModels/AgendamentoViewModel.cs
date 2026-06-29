using PetShopCare.Models;
using PetShopCare.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PetShopCare.ViewModels {
    public class AgendamentoViewModel : ViewModelBase {
        private readonly ClienteService _clienteService;
        private readonly PetService _petService;
        private readonly ServicoService _servicoService;
        private readonly OrdemServicoService _ordemServicoService;

        // --- COLEÇÕES PARA A INTERFACE ---
        public ObservableCollection<Cliente> ClientesDisponiveis { get; set; }
        public ObservableCollection<Pet> PetsDoCliente { get; set; }
        public ObservableCollection<Servico> ServicosDisponiveis { get; set; }
        public ObservableCollection<OrdemServico> AgendamentosDoDia { get; set; }

        // --- PROPRIEDADES DE ESTADO (SELEÇÕES DO UTILIZADOR) ---
        private DateTime _dataSelecionada = DateTime.Now;
        public DateTime DataSelecionada {
            get => _dataSelecionada;
            set {
                _dataSelecionada = value;
                OnPropertyChanged();
                CarregarAgendamentosDoDia(); // Recarrega a agenda ao mudar o dia
            }
        }

        private DateTime _horaSelecionada = DateTime.Now;
        public DateTime HoraSelecionada {
            get => _horaSelecionada;
            set { _horaSelecionada = value; OnPropertyChanged(); }
        }

        private Cliente? _clienteSelecionado;
        public Cliente? ClienteSelecionado {
            get => _clienteSelecionado;
            set {
                _clienteSelecionado = value;
                OnPropertyChanged();
                CarregarPetsDoCliente(); // Cascata: carrega os pets deste tutor
            }
        }

        private Pet? _petSelecionado;
        public Pet? PetSelecionado {
            get => _petSelecionado;
            set { _petSelecionado = value; OnPropertyChanged(); }
        }

        private Servico? _servicoSelecionado;
        public Servico? ServicoSelecionado {
            get => _servicoSelecionado;
            set {
                _servicoSelecionado = value;
                OnPropertyChanged();
                if (_servicoSelecionado != null) {
                    ValorAgendamento = _servicoSelecionado.Preco; // Sugere o preço base do catálogo
                }
            }
        }

        private decimal _valorAgendamento;
        public decimal ValorAgendamento {
            get => _valorAgendamento;
            set { _valorAgendamento = value; OnPropertyChanged(); }
        }

        private string _observacoes = string.Empty;
        public string Observacoes {
            get => _observacoes;
            set { _observacoes = value; OnPropertyChanged(); }
        }

        // --- COMANDOS ---
        public ICommand AgendarCommand { get; }
        public ICommand CancelarAgendamentoCommand { get; } // Novo comando de cancelamento

        public AgendamentoViewModel() {
            _clienteService = new ClienteService();
            _petService = new PetService();
            _servicoService = new ServicoService();
            _ordemServicoService = new OrdemServicoService();

            ClientesDisponiveis = new ObservableCollection<Cliente>();
            PetsDoCliente = new ObservableCollection<Pet>();
            ServicosDisponiveis = new ObservableCollection<Servico>();
            AgendamentosDoDia = new ObservableCollection<OrdemServico>();

            // O botão de agendar só fica ativo se Tutor, Pet e Serviço estiverem preenchidos
            AgendarCommand = new RelayCommand(ExecutarAgendamento, PodeAgendar);

            // Inicialização do comando de cancelamento
            CancelarAgendamentoCommand = new RelayCommand(ExecutarCancelarAgendamento);

            CarregarDadosIniciais();
        }

        private void CarregarDadosIniciais() {
            var clientes = _clienteService.ListarTodos();
            foreach (var c in clientes) ClientesDisponiveis.Add(c);

            var servicos = _servicoService.BuscarTodos();
            foreach (var s in servicos) ServicosDisponiveis.Add(s);

            CarregarAgendamentosDoDia();
        }

        private void CarregarPetsDoCliente() {
            PetsDoCliente.Clear();
            PetSelecionado = null;

            if (ClienteSelecionado != null) {
                // Aqui pressupomos que o seu PetService tem um método para buscar pets por ID do Tutor
                var pets = _petService.ListarTodos().Where(p => p.ClienteId == ClienteSelecionado.Id).ToList();
                foreach (var p in pets) {
                    PetsDoCliente.Add(p);
                }
            }
        }

        private void CarregarAgendamentosDoDia() {
            var agendamentos = _ordemServicoService.BuscarAgendamentosDoDia(DataSelecionada);
            var todosOsPets = _petService.ListarTodos();
            AgendamentosDoDia.Clear();

            foreach (var agendamento in agendamentos) {
                // Como as nossas queries do SQLite ainda não fazem JOIN automático com Clientes, Pets e Serviços,
                // populamos os nomes visuais "na mão" para a DataGrid ficar bonita e amigável.
                agendamento.ClienteNome = ClientesDisponiveis.FirstOrDefault(c => c.Id == agendamento.ClienteId)?.Nome ?? "Desconhecido";
                agendamento.ServicoNome = ServicosDisponiveis.FirstOrDefault(s => s.Id == agendamento.ServicoId)?.Nome ?? "Desconhecido";
                agendamento.PetNome = todosOsPets.FirstOrDefault(p => p.Id == agendamento.PetId)?.Nome ?? "Desconhecido";

                AgendamentosDoDia.Add(agendamento);
            }
        }

        private bool PodeAgendar(object? parameter) {
            return ClienteSelecionado != null && PetSelecionado != null && ServicoSelecionado != null;
        }

        private void ExecutarAgendamento(object? parameter) {
            try {
                // Funde a data do calendário com a hora selecionada
                var dataHoraFinal = DataSelecionada.Date.Add(HoraSelecionada.TimeOfDay);

                var novaOrdem = new OrdemServico {
                    ClienteId = ClienteSelecionado!.Id,
                    PetId = PetSelecionado!.Id,
                    ServicoId = ServicoSelecionado!.Id,
                    DataHoraAgendamento = dataHoraFinal,
                    Valor = ValorAgendamento,
                    Observacoes = Observacoes,
                    Status = "Agendado",
                    UsuarioResponsavelId = 1 // Simulando utilizador logado
                };

                _ordemServicoService.Agendar(novaOrdem);

                System.Windows.MessageBox.Show("Serviço agendado com sucesso!", "Agenda", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                // Limpa o formulário, mas mantém a data atual
                ClienteSelecionado = null;
                ServicoSelecionado = null;
                ValorAgendamento = 0;
                Observacoes = string.Empty;

                // Atualiza a tabela imediatamente
                CarregarAgendamentosDoDia();
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show($"Erro ao agendar: {ex.Message}", "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecutarCancelarAgendamento(object? parameter) {
            if (parameter is OrdemServico agendamento) {
                // Barreira de segurança contra cliques acidentais
                var resultado = System.Windows.MessageBox.Show(
                    $"Deseja realmente cancelar o agendamento do pet {agendamento.PetNome}?",
                    "Confirmar Cancelamento",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (resultado == System.Windows.MessageBoxResult.Yes) {
                    try {
                        _ordemServicoService.CancelarAgendamento(agendamento.Id);

                        // Recarrega a lista da tela para atualizar o status visualmente
                        CarregarAgendamentosDoDia();
                    }
                    catch (Exception ex) {
                        System.Windows.MessageBox.Show($"Erro ao cancelar agendamento: {ex.Message}", "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}