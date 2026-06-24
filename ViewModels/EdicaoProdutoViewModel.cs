using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PetShopCare.Models;
using PetShopCare.Services;

namespace PetShopCare.ViewModels {
    public class EdicaoProdutoViewModel : ViewModelBase {
        private readonly ProdutoService _produtoService;

        public Produto ProdutoEditado { get; set; }
        public ObservableCollection<string> CategoriasDisponiveis { get; set; }

        private string _precoCustoTexto = string.Empty;
        public string PrecoCustoTexto {
            get => _precoCustoTexto;
            set {
                _precoCustoTexto = value;
                string valorLimpo = value?.Replace('.', ',') ?? "0";
                if (decimal.TryParse(valorLimpo, out decimal convertido)) {
                    ProdutoEditado.PrecoCusto = convertido;
                }
                OnPropertyChanged();
            }
        }

        private string _precoVendaTexto = string.Empty;
        public string PrecoVendaTexto {
            get => _precoVendaTexto;
            set {
                _precoVendaTexto = value;
                string valorLimpo = value?.Replace('.', ',') ?? "0";
                if (decimal.TryParse(valorLimpo, out decimal convertido)) {
                    ProdutoEditado.PrecoVenda = convertido;
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

        public EdicaoProdutoViewModel(Produto produtoOriginal) {
            _produtoService = new ProdutoService();

            // CLONAGEM DE ESTADO: Criamos uma cópia exata para não sujar a DataGrid em caso de cancelamento
            ProdutoEditado = new Produto {
                Id = produtoOriginal.Id,
                Nome = produtoOriginal.Nome,
                CodigoBarras = produtoOriginal.CodigoBarras,
                Marca = produtoOriginal.Marca,
                Categoria = produtoOriginal.Categoria,
                Descricao = produtoOriginal.Descricao,
                PrecoCusto = produtoOriginal.PrecoCusto,
                PrecoVenda = produtoOriginal.PrecoVenda,
                EstoqueMinimo = produtoOriginal.EstoqueMinimo,
                EstoqueAtual = produtoOriginal.EstoqueAtual // Apenas repassamos, não editamos na view
            };

            // Preenchemos os campos de texto com a formatação correta
            PrecoCustoTexto = ProdutoEditado.PrecoCusto.ToString("N2");
            PrecoVendaTexto = ProdutoEditado.PrecoVenda.ToString("N2");

            CategoriasDisponiveis = new ObservableCollection<string> {
                "Ração", "Petisco", "Brinquedo", "Higiene", "Medicamento", "Acessório"
            };

            SalvarCommand = new RelayCommand(ExecutarSalvar, PodeSalvar);
            CancelarCommand = new RelayCommand(ExecutarCancelar);
        }

        private bool PodeSalvar(object? p) {
            return !string.IsNullOrWhiteSpace(ProdutoEditado.Nome) &&
                   ProdutoEditado.PrecoVenda > 0;
        }

        private void ExecutarSalvar(object? p) {
            try {
                // Como o ProdutoEditado tem um Id > 0, o ProdutoService fará um UPDATE
                _produtoService.SalvarProduto(ProdutoEditado);
                FecharJanela?.Invoke();
            }
            catch (Exception ex) {
                MensagemErro = ex.Message;
            }
        }

        private void ExecutarCancelar(object? p) {
            FecharJanela?.Invoke();
        }
    }
}