using PetShopCare.ViewModels;
using System.Windows.Controls;

namespace PetShopCare.Views {
    public partial class DashboardView : UserControl {
        public DashboardView() {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }
    }
}