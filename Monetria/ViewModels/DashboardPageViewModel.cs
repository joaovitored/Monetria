using System;
using System.Collections.ObjectModel;
using System.Linq;
using Monetria.Models;
using Monetria.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace Monetria.ViewModels
{
    public class DashboardPageViewModel : ViewModelBase
    {
        private readonly TransacaoService _service;

        // gráficos
        public ObservableCollection<ISeries> PieSeries { get; } = new();
        public ObservableCollection<ISeries> LineSeries { get; } = new();

        //eixos para gráfico de linha
        public ObservableCollection<Axis> XAxes { get; } = new();
        public ObservableCollection<Axis> YAxes { get; } = new();

        //construtor obrigatório recebendo o serviço
        public DashboardPageViewModel(TransacaoService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));

            //atualiza gráficos sempre que a coleção de transações muda
            _service.Transacoes.CollectionChanged += (_, _) => AtualizarGraficos();

            //atualiza gráficos na inicialização
            AtualizarGraficos();
        }

        private void AtualizarGraficos()
        {
            AtualizarPie();
            AtualizarLine();
        }

        private void AtualizarPie()
        {
            PieSeries.Clear();

            var grupos = _service.Transacoes
                .GroupBy(t => t.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(t => t.Valor) })
                .ToList();

            foreach (var g in grupos)
            {
                PieSeries.Add(new PieSeries<double>
                {
                    Name = g.Categoria,
                    Values = new[] { (double)g.Total }
                });
            }

            // adiciona total apenas na legenda
            var total = _service.Transacoes.Sum(t => t.Valor);
            PieSeries.Add(new PieSeries<double>
            {
                Values = new double[] { 0 },
                Name = $"Total: R$ {total:N2}",
                Fill = null,
                DataLabelsSize = 0,
                IsVisible = true
            });
        }

        private void AtualizarLine()
        {
            LineSeries.Clear();
            XAxes.Clear();
            YAxes.Clear();

            //agrupa por mês e ano, mas sem acumular valores
            var agrupado = _service.Transacoes
                .OrderBy(t => t.Data)
                .GroupBy(t => new { t.Data.Year, t.Data.Month })
                .Select(g => new
                {
                    Ano = g.Key.Year,
                    Mes = g.Key.Month,
                    TotalMes = g.Sum(t => t.Tipo == "Income" ? (double)t.Valor : -(double)t.Valor)
                })
                .ToList();

            //valores para gráfico (apenas total do mês)
            LineSeries.Add(new LineSeries<double>
            {
                Values = agrupado.Select(a => a.TotalMes).ToArray(),
                Name = "Monthly Evolution ($)"
            });

            //labels X (ex: Jan/2026)
            XAxes.Add(new Axis
            {
                Labels = agrupado.Select(a => new DateTime(a.Ano, a.Mes, 1).ToString("MMM/yyyy")).ToArray()
            });

            //eixo Y automático
            YAxes.Add(new Axis());
        }
    }
}
