using Monetria.Models;
using Monetria.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Monetria.ViewModels
{
    public class RelatorioPageViewModel : ViewModelBase
    {
        private readonly TransacaoService _service;

        // Séries de gráficos
        public ObservableCollection<ISeries> PieSeries { get; } = new();
        public ObservableCollection<ISeries> BarSeries { get; } = new();
        public ObservableCollection<ISeries> LineSeries { get; } = new();

        // Eixos para o gráfico de linha
        public ObservableCollection<Axis> XAxes { get; } = new();
        public ObservableCollection<Axis> YAxes { get; } = new();

        // Filtros
        private string _mesSelecionado = string.Empty; // Inicialização com string vazia
        private int _anoSelecionado;

        // Listas de meses e anos disponíveis
        public ObservableCollection<string> MesesDisponiveis { get; } = new();
        public ObservableCollection<int> AnosDisponiveis { get; } = new();

        public string MesSelecionado
        {
            get => _mesSelecionado;
            set
            {
                if (SetProperty(ref _mesSelecionado, value))
                    FiltrarTransacoes();
            }
        }

        public int AnoSelecionado
        {
            get => _anoSelecionado;
            set
            {
                if (SetProperty(ref _anoSelecionado, value))
                    FiltrarTransacoes();
            }
        }

        // Construtor
        public RelatorioPageViewModel()
        {
            _service = AppServices.TransacaoService;

            // Atualiza quando as transações mudam (adicionar novas transações)
            _service.Transacoes.CollectionChanged += (_, _) => Atualizar();

            // Carregar meses e anos disponíveis no início
            CarregarMesesEAno();

            // Inicializa a tela com o primeiro mês disponível e o ano atual
            if (MesesDisponiveis.Any())
                MesSelecionado = MesesDisponiveis.First();  // Define o primeiro mês disponível

            if (AnosDisponiveis.Any())
                AnoSelecionado = DateTime.Now.Year;
        }

        // Carregar meses e anos únicos com base nas transações
        private void CarregarMesesEAno()
        {
            var meses = _service.Transacoes
                .Select(t => t.Data.ToString("MMMM"))
                .Distinct()
                .OrderBy(m => DateTime.ParseExact(m, "MMMM", System.Globalization.CultureInfo.CurrentCulture))  // Ordena os meses
                .ToList();

            var anos = _service.Transacoes
                .Select(t => t.Data.Year)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            // Preencher as listas de meses e anos
            MesesDisponiveis.Clear();
            foreach (var mes in meses)
                MesesDisponiveis.Add(mes);

            AnosDisponiveis.Clear();
            foreach (var ano in anos)
                AnosDisponiveis.Add(ano);

            // Definir o mês e ano selecionados (opcionais, para inicialização)
            if (MesesDisponiveis.Any())
                MesSelecionado = MesesDisponiveis.First();  // Define o primeiro mês disponível

            if (AnosDisponiveis.Any())
                AnoSelecionado = AnosDisponiveis.First();
        }

        // Filtra as transações com base no mês e ano selecionado
        private void FiltrarTransacoes()
        {
            if (string.IsNullOrEmpty(MesSelecionado) || AnoSelecionado == 0)
                return;

            // Normaliza o nome do mês (primeira letra maiúscula)
            var mesSelecionadoNormalizado = char.ToUpper(MesSelecionado[0]) + MesSelecionado.Substring(1).ToLower();

            // Converter o nome do mês para número
            var mesNumero = Array.IndexOf(
                new[] { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" },
                mesSelecionadoNormalizado) + 1;

            if (mesNumero == 0)
            {
                Console.WriteLine($"Erro: Mês '{MesSelecionado}' não encontrado.");
                return;  // Se o mês não for válido, retornamos
            }

            // Filtra as transações com base no mês e no ano
            var transacoesFiltradas = _service.Transacoes
                .Where(t => t.Data.Month == mesNumero && t.Data.Year == AnoSelecionado)
                .ToList();

            // Log para depuração
            Console.WriteLine($"Mês Selecionado: {MesSelecionado}, Ano Selecionado: {AnoSelecionado}");
            Console.WriteLine($"Transações filtradas: {transacoesFiltradas.Count}");

            // Atualiza os gráficos com as transações filtradas
            AtualizarGraficos(transacoesFiltradas);
        }

        // Atualiza quando houver mudanças nas transações ou nos filtros
        private void Atualizar()
        {
            // Carregar novamente os meses e anos com as transações mais recentes
            CarregarMesesEAno();

            // Filtrar as transações novamente com os dados mais recentes
            FiltrarTransacoes();
        }

        // Atualiza os gráficos com as transações filtradas
        private void AtualizarGraficos(List<Transacao> transacoes)
        {
            // Atualiza o gráfico de pizza (despesas por categoria)
            PieSeries.Clear();
            var despesas = transacoes
                .Where(t => t.Tipo == "Despesa")
                .GroupBy(t => t.Categoria)
                .Select(g => new { g.Key, Total = g.Sum(t => t.Valor) })
                .ToList();

            // Log para depuração
            Console.WriteLine($"Despesas por categoria: {despesas.Count} categorias encontradas.");

            foreach (var g in despesas)
            {
                PieSeries.Add(new PieSeries<double>
                {
                    Name = g.Key,
                    Values = new[] { (double)g.Total }
                });
            }

            // Atualiza o gráfico de barras (receitas e despesas)
            BarSeries.Clear();
            var totalReceitas = transacoes.Where(t => t.Tipo == "Income").Sum(t => t.Valor);
            var totalDespesas = transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);

            // Log para depuração
            Console.WriteLine($"Total Receitas: {totalReceitas}, Total Despesas: {totalDespesas}");

            BarSeries.Add(new ColumnSeries<double>
            {
                Name = "Receitas",
                Values = new[] { (double)totalReceitas }
            });

            BarSeries.Add(new ColumnSeries<double>
            {
                Name = "Despesas",
                Values = new[] { (double)totalDespesas }
            });

            // Atualiza o gráfico de linha (evolução do saldo)
            LineSeries.Clear();
            XAxes.Clear();
            YAxes.Clear();

            double saldo = 0;
            var valores = _service.Transacoes
                .OrderBy(t => t.Data)  // Ordenar as transações pela data
                .GroupBy(t => new { t.Data.Year, t.Data.Month })  // Agrupar por ano e mês
                .Select(g =>
                {
                    saldo += g.Sum(t => t.Tipo == "Income" ? (double)t.Valor : -(double)t.Valor);
                    return saldo;
                })
                .ToArray();

            // Log para depuração
            Console.WriteLine($"Valores para gráfico de linha: {valores.Length} valores.");

            LineSeries.Add(new LineSeries<double> { Values = valores });

            // Eixo X: nomes dos meses de todos os anos disponíveis
            var meses = _service.Transacoes
                .OrderBy(t => t.Data)
                .GroupBy(t => t.Data.ToString("MMM"))
                .Select(g => g.Key)
                .Distinct()
                .ToArray();

            // Log para depuração
            Console.WriteLine($"Meses para o eixo X: {string.Join(", ", meses)}");

            XAxes.Add(new Axis { Labels = meses });
            YAxes.Add(new Axis()); // Eixo Y automático
        }
    }
}
