using PetShopCare.Models;
using PetShopCare.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using PetShopCare.Repositories;

namespace PetShopCare.ViewModels {
    public class DashboardViewModel : ViewModelBase {
        private readonly VendaRepository _vendaRepository;
        private readonly OrdemServicoService _ordemServicoService;
        private readonly ProdutoService _produtoService;

        private decimal _faturamentoHoje;
        public decimal FaturamentoHoje {
            get => _faturamentoHoje;
            set { _faturamentoHoje = value; OnPropertyChanged(); }
        }

        private int _servicosHoje;
        public int ServicosHoje {
            get => _servicosHoje;
            set { _servicosHoje = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Produto> ProdutosEstoqueCritico { get; set; }

        // --- PROPRIEDADES DO GRÁFICO LIVECHARTS ---
        public ISeries[] FaturamentoSeries { get; set; }
        public Axis[] EixoX { get; set; }
        public Axis[] EixoY { get; set; }

        public DashboardViewModel() {
            _vendaRepository = new VendaRepository();
            _ordemServicoService = new OrdemServicoService();
            _produtoService = new ProdutoService();
            ProdutosEstoqueCritico = new ObservableCollection<Produto>();

            CarregarMetricas();
            ConfigurarGrafico();
        }

        public void CarregarMetricas() {
            var hoje = DateTime.Today;

            // Faturamento Hoje
            var vendasHoje = _vendaRepository.BuscarTodas().Where(v => v.DataVenda.Date == hoje).ToList();
            FaturamentoHoje = vendasHoje.Sum(v => v.ValorTotal);

            // Serviços Agendados/Dia
            var agendamentosDoDia = _ordemServicoService.BuscarAgendamentosDoDia(hoje);
            ServicosHoje = agendamentosDoDia.Count;

            // Estoque Crítico
            ProdutosEstoqueCritico.Clear();
            var todosProdutos = _produtoService.ListarTodos();
            foreach (var p in todosProdutos.Where(p => p.EstoqueAtual <= p.EstoqueMinimo)) {
                ProdutosEstoqueCritico.Add(p);
            }
        }

        private void ConfigurarGrafico() {
            // Simulando faturamento dos últimos 7 dias para o gráfico
            // Num cenário real, você faria um GROUP BY pela data no banco de dados
            var valoresFaturamento = new double[] { 1200, 1500, 900, 2100, 1800, 2500, (double)FaturamentoHoje };
            var ultimos7Dias = new[] {
                DateTime.Now.AddDays(-6).ToString("dd/MM"),
                DateTime.Now.AddDays(-5).ToString("dd/MM"),
                DateTime.Now.AddDays(-4).ToString("dd/MM"),
                DateTime.Now.AddDays(-3).ToString("dd/MM"),
                DateTime.Now.AddDays(-2).ToString("dd/MM"),
                DateTime.Now.AddDays(-1).ToString("dd/MM"),
                "Hoje"
            };

            // Linha do Gráfico
            FaturamentoSeries = new ISeries[] {
                new LineSeries<double> {
                    Values = valoresFaturamento,
                    Name = "Faturamento (R$)",
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue) { StrokeThickness = 4 },
                    Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(50)), // Sombra debaixo da linha
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    GeometryStroke = new SolidColorPaint(SKColors.DodgerBlue) { StrokeThickness = 3 },
                    LineSmoothness = 0.5 // Curva suave
                }
            };

            // Configuração dos Eixos
            EixoX = new Axis[] {
                new Axis {
                    Labels = ultimos7Dias,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 14
                }
            };

            EixoY = new Axis[] {
                new Axis {
                    Labeler = value => value.ToString("C2"), // Formata como Moeda (R$)
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 14
                }
            };
        }
    }
}