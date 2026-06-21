using System.Windows.Controls;
using PetShopCare.ViewModels;

namespace PetShopCare.Views {
    public partial class PetView : UserControl {
        public PetView() {
            InitializeComponent();

            // Injeta o controlador lógico logo que o ecrã é desenhado
            this.DataContext = new PetViewModel();
        }
    }
}