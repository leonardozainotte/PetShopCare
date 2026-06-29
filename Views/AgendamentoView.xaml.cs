using PetShopCare.ViewModels;
using System.Windows.Controls;

namespace PetShopCare.Views {
    public partial class AgendamentoView : UserControl {
        public AgendamentoView() {
            InitializeComponent();
            DataContext = new AgendamentoViewModel();
        }
    }
}