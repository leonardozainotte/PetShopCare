using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class CadastroProdutoViewModel : ViewModelBase {
        private readonly ProdutoService _produtoService;

        public Produto NovoProduto { get; set; }
        public ObservableCollection<string> CategoriasDisponiveis { get; set; }

        // Intermediário para Preço de Custo (Tratamento de vírgula/ponto)
        private string _precoCustoTexto = string.Empty;
        public string PrecoCustoTexto {
            get => _precoCustoTexto;
            set {
                _precoCustoTexto = value;
                string valorLimpo = value?.Replace('.', ',') ?? "0";
                if (decimal.TryParse(valorLimpo, out decimal convertido)) {
                    NovoProduto.PrecoCusto = convertido;
                }
                OnPropertyChanged();
            }
        }

        // Intermediário para Preço de Venda
        private string _precoVendaTexto = string.Empty;
        public string PrecoVendaTexto {
            get => _precoVendaTexto;
            set {
                _precoVendaTexto = value;
                string valorLimpo = value?.Replace('.', ',') ?? "0";
                if (decimal.TryParse(valorLimpo, out decimal convertido)) {
                    NovoProduto.PrecoVenda = convertido;
                }
                OnPropertyChanged();
            }
        }

        private string _mensagemErro = string.Empty;
        public string MensagemErro {
            get => _mensagemErro;
            set { _mensagemErro = value; OnPropertyChanged(); }
        }

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }
        public Action? FecharJanela { get; set; }

        public CadastroProdutoViewModel() {
            _produtoService = new ProdutoService();
            NovoProduto = new Produto();

            CategoriasDisponiveis = new ObservableCollection<string> {
                "Ração", "Petisco", "Brinquedo", "Higiene", "Medicamento", "Acessório"
            };

            SalvarCommand = new RelayCommand(ExecutarSalvar, PodeSalvar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
        }

        private bool PodeSalvar(object? p) {
            return !string.IsNullOrWhiteSpace(NovoProduto.Nome) &&
                   NovoProduto.PrecoVenda > 0;
        }

        private void ExecutarSalvar(object? p) {
            try {
                _produtoService.SalvarProduto(NovoProduto);
                FecharJanela?.Invoke();
            }
            catch (Exception ex) {
                MensagemErro = ex.Message; // Exibirá o erro de bloqueio financeiro se houver
            }
        }

        private void ExecutarCancelar(object? p) {
            FecharJanela?.Invoke();
        }
    }
}