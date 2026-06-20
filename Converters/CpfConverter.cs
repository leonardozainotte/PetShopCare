using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PetShopCare.Converters {
    public class CpfConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string texto) {
                // Extrai apenas os números para garantir segurança na conversão
                var numeros = new string(texto.Where(char.IsDigit).ToArray());

                // Formato: 999.999.999-99
                if (numeros.Length == 11)
                    return $"{numeros.Substring(0, 3)}.{numeros.Substring(3, 3)}.{numeros.Substring(6, 3)}-{numeros.Substring(9, 2)}";
            }
            return value; // Se o dado estiver malformado, devolve o original para não ocultar o erro
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}