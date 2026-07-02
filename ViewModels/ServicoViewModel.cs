using PetShopCare.Models;
using PetShopCare.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PetShopCare.ViewModels {
    public class ServicoViewModel : ViewModelBase {
        private readonly ServicoService _servicoService;
        public ObservableCollection<Servico> Servicos { get; set; }

        public ICommand AbrirCadastroCommand { get; }
        public ICommand AbrirEdicaoCommand { get; }
        public ICommand ExcluirCommand { get; }

        public ServicoViewModel() {
            _servicoService = new ServicoService();
            Servicos = new ObservableCollection<Servico>();

            AbrirCadastroCommand = new RelayCommand(ExecutarAbrirCadastro);
            AbrirEdicaoCommand = new RelayCommand(ExecutarAbrirEdicao);
            ExcluirCommand = new RelayCommand(ExecutarExcluir);

            CarregarServicos();
        }

        public void CarregarServicos() {
            Servicos.Clear();
            var lista = _servicoService.BuscarTodos();
            foreach (var servico in lista) {
                Servicos.Add(servico);
            }
        }

        private void ExecutarAbrirCadastro(object obj) {
            // Abriremos a janela de Cadastro (que criaremos no próximo passo)
            var janelaCadastro = new Views.CadastroServicoView();
            var viewModel = new CadastroServicoViewModel(this); // Passamos 'this' para ele poder atualizar a lista depois de salvar
            janelaCadastro.DataContext = viewModel;
            janelaCadastro.ShowDialog();
        }

        private void ExecutarAbrirEdicao(object obj) {
            if (obj is Servico servicoSelecionado) {
                // Reutilizamos a mesma janela de Cadastro, mas passando o objeto para edição
                var janelaEdicao = new Views.CadastroServicoView();
                var viewModel = new CadastroServicoViewModel(this, servicoSelecionado);
                janelaEdicao.DataContext = viewModel;
                janelaEdicao.ShowDialog();
            }
        }

        private void ExecutarExcluir(object obj) {
            if (obj is Servico servicoSelecionado) {
                var resposta = MessageBox.Show($"Tem certeza que deseja excluir o serviço '{servicoSelecionado.Nome}'?",
                                               "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (resposta == MessageBoxResult.Yes) {
                    _servicoService.Excluir(servicoSelecionado.Id);
                    CarregarServicos(); // Atualiza a tabela instantaneamente
                }
            }
        }
    }
}