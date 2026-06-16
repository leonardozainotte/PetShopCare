using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetShopCare.ViewModels {
    // A classe abstrata serve como molde. Todas as nossas ViewModels vão herdar dela.
    public abstract class ViewModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        // O [CallerMemberName] descobre automaticamente qual variável chamou o método,
        // evitando que tenhamos que digitar o nome dela em texto (evita erros de digitação).
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}