using System.Windows;
using PetShopCare.ViewModels;

namespace PetShopCare.Views {
    public partial class EdicaoPetView : Window {
        public EdicaoPetView() {
            InitializeComponent();

            this.DataContextChanged += (s, e) => {
                if (this.DataContext is EdicaoPetViewModel viewModel) {
                    viewModel.FecharJanela = () => {
                        try { this.DialogResult = true; } catch { }
                        this.Close();
                    };
                }
            };
        }
    }
}