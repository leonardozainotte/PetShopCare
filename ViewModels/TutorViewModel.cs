using System;
using System.Collections.ObjectModel;
using System.Windows; // Necessário para Application.Current.MainWindow
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;
using PetShopCare.Views;

namespace PetShopCare.ViewModels
{
    public class TutorViewModel : ViewModelBase
    {
        private readonly ClienteService _clienteService;
        private ObservableCollection<Cliente> _tutores;

        // Propriedade pública que o DataGrid do XAML vai monitorizar
        public ObservableCollection<Cliente> Tutores
        {
            get => _tutores;
            set
            {
                _tutores = value;
                OnPropertyChanged();
            }
        }

        // Comandos de navegação
        public ICommand AbrirCadastroCommand { get; }
        public ICommand AbrirEdicaoCommand { get; } // <-- O comando do lápis agora existe

        public TutorViewModel()
        {
            // Inicializa o serviço de regras de negócio
            _clienteService = new ClienteService();

            // Inicializa a coleção na memória
            Tutores = new ObservableCollection<Cliente>();

            // Configuração dos gatilhos utilizando o RelayCommand
            AbrirCadastroCommand = new RelayCommand(ExecutarAbrirCadastro);
            AbrirEdicaoCommand = new RelayCommand(ExecutarAbrirEdicao); // <-- Gatilho ativado

            CarregarTutores();
        }

        private void CarregarTutores()
        {
            try
            {
                var dadosDoBanco = _clienteService.ListarTodos();

                if (dadosDoBanco != null)
                {
                    Tutores = new ObservableCollection<Cliente>(dadosDoBanco);
                }
            }
            catch (Exception)
            {
                // Tratamento de falhas estruturais
            }
        }

        private void ExecutarAbrirCadastro(object parameter)
        {
            // MODO NOVO CADASTRO: Passamos 'null' para a ViewModel híbrida
            var viewModelModal = new CadastroTutorViewModel(null);
            AbrirJanelaModal(viewModelModal);
        }

        private void ExecutarAbrirEdicao(object parameter)
        {
            if (parameter is Cliente clienteSelecionado)
            {
                // 1. Instancia a nova ViewModel exclusiva de edição
                var viewModelModal = new EdicaoTutorViewModel(clienteSelecionado);

                // 2. Abre a nova Janela de edição
                var viewModal = new Views.EdicaoTutorView { DataContext = viewModelModal };

                if (Application.Current.MainWindow != null)
                {
                    viewModal.Owner = Application.Current.MainWindow;
                }

                viewModal.ShowDialog();

                // 3. Atualiza a tabela após fechar
                CarregarTutores();
            }
        }

        // Método auxiliar para não repetirmos código ao abrir a janela
        private void AbrirJanelaModal(CadastroTutorViewModel viewModelModal)
        {
            // Criamos a janela e injetamos o "cérebro" (DataContext) correto nela
            var modalCadastro = new CadastroTutorView
            {
                DataContext = viewModelModal
            };

            // Centraliza a modal em relação à janela principal (Dashboard)
            if (Application.Current.MainWindow != null)
            {
                modalCadastro.Owner = Application.Current.MainWindow;
            }

            modalCadastro.ShowDialog();

            // Independentemente de o operador ter salvo, alterado ou excluído,
            // recarregamos a tabela para refletir a base de dados instantaneamente
            CarregarTutores();
        }
    }
}