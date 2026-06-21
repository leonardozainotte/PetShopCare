using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;
using PetShopCare.Views;

namespace PetShopCare.ViewModels {
    public class PetViewModel : ViewModelBase {
        private readonly PetService _petService;
        private ObservableCollection<Pet> _pets;

        // A lista que a DataGrid vai observar
        public ObservableCollection<Pet> Pets {
            get => _pets;
            set {
                _pets = value;
                OnPropertyChanged();
            }
        }

        // Comandos para os botões de ação
        public ICommand AbrirCadastroCommand { get; }
        public ICommand AbrirEdicaoCommand { get; }

        public PetViewModel() {
            _petService = new PetService();
            Pets = new ObservableCollection<Pet>();

            AbrirCadastroCommand = new RelayCommand(ExecutarAbrirCadastro);
            AbrirEdicaoCommand = new RelayCommand(ExecutarAbrirEdicao);

            CarregarPets();
        }

        private void CarregarPets() {
            try {
                var dados = _petService.ListarTodos();
                if (dados != null) {
                    Pets = new ObservableCollection<Pet>(dados);
                }
            }
            catch (Exception) {
                // Em um cenário real, poderíamos logar este erro
            }
        }

        private void ExecutarAbrirCadastro(object parameter) {
            // 1. Prepara o cérebro da janela de cadastro
            var viewModel = new CadastroPetViewModel();

            // 2. Prepara a janela visual e injeta o cérebro nela
            var janela = new CadastroPetView {
                DataContext = viewModel
            };

            // 3. Ensina o ViewModel como fechar esta janela específica
            viewModel.FecharJanela = () => janela.Close();

            // 4. Abre a janela e bloqueia a tela de trás (Modal)
            janela.ShowDialog();

            // 5. GATILHO DE SINCRONIZAÇÃO: Assim que a janela fechar, a tabela atualiza!
            CarregarPets();
        }

        private void ExecutarAbrirEdicao(object parameter) {
            if (parameter is Pet petSelecionado) {
                var viewModel = new EdicaoPetViewModel(petSelecionado);
                var janela = new EdicaoPetView {
                    DataContext = viewModel
                };

                viewModel.FecharJanela = () => janela.Close();
                janela.ShowDialog();

                // Recarrega os dados do banco após a janela de edição ser fechada
                CarregarPets();
            }
        }
    }
}