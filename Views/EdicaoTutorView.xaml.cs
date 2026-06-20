using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PetShopCare.ViewModels;

namespace PetShopCare.Views {
    public partial class EdicaoTutorView : Window {
        public EdicaoTutorView() {
            InitializeComponent();

            this.DataContextChanged += (s, e) => {
                if (this.DataContext is EdicaoTutorViewModel viewModel) {
                    viewModel.FecharJanela = () => {
                        try { this.DialogResult = true; } catch { }
                        this.Close();
                    };
                }
            };
        }

        private void CpfTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox) {
                string apenasNumeros = new string(textBox.Text.Where(char.IsDigit).ToArray());
                if (apenasNumeros.Length > 11) apenasNumeros = apenasNumeros.Substring(0, 11);

                string formatado = apenasNumeros;
                if (apenasNumeros.Length >= 10) formatado = $"{apenasNumeros.Substring(0, 3)}.{apenasNumeros.Substring(3, 3)}.{apenasNumeros.Substring(6, 3)}-{apenasNumeros.Substring(9)}";
                else if (apenasNumeros.Length >= 7) formatado = $"{apenasNumeros.Substring(0, 3)}.{apenasNumeros.Substring(3, 3)}.{apenasNumeros.Substring(6)}";
                else if (apenasNumeros.Length >= 4) formatado = $"{apenasNumeros.Substring(0, 3)}.{apenasNumeros.Substring(3)}";

                if (textBox.Text != formatado) {
                    textBox.Text = formatado;
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }
        }

        private void TelefoneTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox) {
                string apenasNumeros = new string(textBox.Text.Where(char.IsDigit).ToArray());
                if (apenasNumeros.Length > 11) apenasNumeros = apenasNumeros.Substring(0, 11);

                string formatado = apenasNumeros;
                if (apenasNumeros.Length == 11) formatado = $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2, 5)}-{apenasNumeros.Substring(7)}";
                else if (apenasNumeros.Length >= 7) formatado = $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2, 4)}-{apenasNumeros.Substring(6)}";
                else if (apenasNumeros.Length >= 3) formatado = $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2)}";

                if (textBox.Text != formatado) {
                    textBox.Text = formatado;
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }
        }
    }
}