using System.Windows;
using PetShopCare.ViewModels;

namespace PetShopCare.Views {
    public partial class CadastroPetView : Window {
        public CadastroPetView() {
            InitializeComponent();

            // Captura o contexto de criação externo para gerenciar o encerramento da janela
            this.DataContextChanged += (s, e) => {
                if (this.DataContext is CadastroPetViewModel viewModel) {
                    viewModel.FecharJanela = () => {
                        try { this.DialogResult = true; } catch { }
                        this.Close();
                    };
                }
            };
        }
    }
}