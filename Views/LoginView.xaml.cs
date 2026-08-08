using PetShopCare.ViewModels;
using System.Windows;

namespace PetShopCare.Views {
    public partial class LoginView : Window {
        public LoginView() {
            InitializeComponent();

            this.DataContextChanged += (s, e) => {
                if (this.DataContext is LoginViewModel viewModel) {
                    // Quando o ViewModel avisar que o login teve sucesso, fechamos a janela de Login
                    viewModel.AoAutenticarComSucesso = (usuarioLogado) => {
                        this.DialogResult = true;
                        this.Close();
                    };
                }
            };
        }
    }
}