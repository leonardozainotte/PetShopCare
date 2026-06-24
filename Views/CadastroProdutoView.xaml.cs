using System.Windows;
using PetShopCare.ViewModels;

namespace PetShopCare.Views {
    public partial class CadastroProdutoView : Window {
        public CadastroProdutoView() {
            InitializeComponent();

            // Intercepta a atribuição do DataContext para amarrar a ação de fechamento
            this.DataContextChanged += (s, e) => {
                if (this.DataContext is CadastroProdutoViewModel viewModel) {
                    viewModel.FecharJanela = () => {
                        // O bloco try/catch protege a atribuição do DialogResult caso a janela 
                        // tenha sido aberta com .Show() em vez de .ShowDialog()
                        try { this.DialogResult = true; } catch { }

                        this.Close();
                    };
                }
            };
        }
    }
}