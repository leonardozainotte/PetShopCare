using System;
using System.Collections.ObjectModel;
using PetShopCare.Models;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class TutorViewModel : ViewModelBase {
        private readonly ClienteService _clienteService;
        private ObservableCollection<Cliente> _tutores;

        // Propriedade pública que o DataGrid do XAML vai monitorizar
        public ObservableCollection<Cliente> Tutores {
            get => _tutores;
            set {
                _tutores = value;
                OnPropertyChanged();
            }
        }

        public TutorViewModel() {
            // Inicializa o serviço de regras de negócio
            _clienteService = new ClienteService();

            // Inicializa a coleção na memória
            Tutores = new ObservableCollection<Cliente>();

            CarregarTutores();
        }

        private void CarregarTutores() {
            try {
                // Nota: Ajuste o nome do método ('ObterTodos' ou 'Listar') conforme definido no seu ClienteService
                var dadosDoBanco = _clienteService.ListarTodos();

                if (dadosDoBanco != null) {
                    Tutores = new ObservableCollection<Cliente>(dadosDoBanco);
                }
            }
            catch (Exception) {
                // Tratamento de falhas estruturais de carregamento (ex: base de dados offline)
                // Para testes iniciais, caso a tabela esteja vazia, pode injetar dados fictícios aqui
            }
        }
    }
}