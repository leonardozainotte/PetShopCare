using System.Windows;
using PetShopCare.ViewModels;

namespace PetShopCare.Views {
    public partial class EdicaoProdutoView : Window {
        public EdicaoProdutoView() {
            InitializeComponent();
            this.DataContextChanged += (s, e) => {
                if (this.DataContext is EdicaoProdutoViewModel viewModel) {
                    viewModel.FecharJanela = () => {
                        try { this.DialogResult = true; } catch { }
                        this.Close();
                    };
                }
            };
        }
    }
}