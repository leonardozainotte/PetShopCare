using System.Windows.Input;

namespace PetShopCare.ViewModels {
    public class MainViewModel : ViewModelBase {
        private object _telaAtual;

        // Propriedade que o ContentControl do XAML monitora para redesenhar a tela central
        public object TelaAtual {
            get { return _telaAtual; }
            set {
                _telaAtual = value;
                OnPropertyChanged(); // Dispara o aviso de atualização para a View
            }
        }

        // Comando que será amarrado ao botão "Tutores" da barra lateral
        public ICommand AbrirTutoresCommand { get; }

        public MainViewModel() {
            // Mensagem de boas-vindas padrão ao iniciar o sistema
            TelaAtual = "Bem-vindo ao PetShopCare! Selecione um menu na barra lateral.";

            // Instancia o comando de navegação, injetando a TutorViewModel na área de trabalho
            AbrirTutoresCommand = new RelayCommand(o => TelaAtual = new TutorViewModel());
        }
    }
}