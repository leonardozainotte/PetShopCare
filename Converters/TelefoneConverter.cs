using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PetShopCare.Converters {
    public class TelefoneConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string texto) {
                var numeros = new string(texto.Where(char.IsDigit).ToArray());

                // Formato Celular: (99) 99999-9999
                if (numeros.Length == 11)
                    return $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 5)}-{numeros.Substring(7, 4)}";

                // Formato Fixo antigo: (99) 9999-9999
                if (numeros.Length == 10)
                    return $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 4)}-{numeros.Substring(6, 4)}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}