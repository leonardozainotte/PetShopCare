using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;
using PetShopCare.Views;

namespace PetShopCare.ViewModels
{
    public class PetViewModel : ViewModelBase
    {
        private readonly PetService _petService;

        // A lista exibida na DataGrid
        private ObservableCollection<Pet> _pets;
        public ObservableCollection<Pet> Pets
        {
            get => _pets;
            set
            {
                _pets = value;
                OnPropertyChanged();
            }
        }

        // 1. Propriedade da barra de pesquisa
        private string _textoPesquisa = string.Empty;
        public string TextoPesquisa
        {
            get => _textoPesquisa;
            set
            {
                _textoPesquisa = value;
                OnPropertyChanged();
                AplicarFiltro(); // Filtra a tabela a cada caractere digitado
            }
        }

        // 2. Backup da lista completa (evita ir ao banco de dados repetidas vezes)
        private List<Pet> _listaCompletaPets = new List<Pet>();

        public ICommand AbrirCadastroCommand { get; }
        public ICommand AbrirEdicaoCommand { get; }

        public PetViewModel()
        {
            _petService = new PetService();
            Pets = new ObservableCollection<Pet>();

            AbrirCadastroCommand = new RelayCommand(ExecutarAbrirCadastro);
            AbrirEdicaoCommand = new RelayCommand(ExecutarAbrirEdicao);

            CarregarPets();
        }

        private void CarregarPets()
        {
            try
            {
                var dados = _petService.ListarTodos();
                if (dados != null)
                {
                    _listaCompletaPets = dados; // Atualiza o backup
                    AplicarFiltro(); // Aplica o filtro antes de mostrar na tela
                }
            }
            catch (Exception)
            {
                // Em um cenário real, poderíamos logar este erro
            }
        }

        // 3. O Cérebro da Filtragem (LINQ)
        private void AplicarFiltro()
        {
            // Se a barra estiver vazia, exibe todos os pets
            if (string.IsNullOrWhiteSpace(TextoPesquisa))
            {
                Pets = new ObservableCollection<Pet>(_listaCompletaPets);
                return;
            }

            var textoDigitado = TextoPesquisa.ToLower();

            // Busca inteligente: avalia nome do pet ou nome do dono
            var filtrados = _listaCompletaPets.Where(p =>
                (p.Nome != null && p.Nome.ToLower().Contains(textoDigitado)) ||
                (p.ClienteNome != null && p.ClienteNome.ToLower().Contains(textoDigitado))
            ).ToList();

            Pets = new ObservableCollection<Pet>(filtrados);
        }

        private void ExecutarAbrirCadastro(object parameter)
        {
            var viewModel = new CadastroPetViewModel();
            var janela = new CadastroPetView
            {
                DataContext = viewModel
            };

            viewModel.FecharJanela = () => janela.Close();
            janela.ShowDialog();

            CarregarPets();
        }

        private void ExecutarAbrirEdicao(object parameter)
        {
            if (parameter is Pet petSelecionado)
            {
                var viewModel = new EdicaoPetViewModel(petSelecionado);
                var janela = new EdicaoPetView
                {
                    DataContext = viewModel
                };

                viewModel.FecharJanela = () => janela.Close();
                janela.ShowDialog();

                CarregarPets();
            }
        }
    }
}