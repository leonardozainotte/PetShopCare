using System.Windows;
using PetShopCare.ViewModels;

namespace PetShopCare.Views {
    public partial class LancarEstoqueView : Window {
        public LancarEstoqueView() {
            InitializeComponent();

            // Intercepta a injeção do ViewModel para amarrar a ação de fechamento corretamente
            this.DataContextChanged += (s, e) => {
                if (this.DataContext is LancarEstoqueViewModel viewModel) {
                    viewModel.FecharJanela = () => {
                        try { this.DialogResult = true; } catch { }
                        this.Close();
                    };
                }
            };
        }
    }
}