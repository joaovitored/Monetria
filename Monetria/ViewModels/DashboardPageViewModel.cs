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

        // Gráficos
        public ObservableCollection<ISeries> PieSeries { get; } = new();
        public ObservableCollection<ISeries> LineSeries { get; } = new();

        public DashboardPageViewModel(TransacaoService service)
        {
            _service = service;

            // Atualiza gráficos sempre que a coleção de transações muda
            _service.Transacoes.CollectionChanged += (s, e) => AtualizarGraficos();

            // Atualiza gráficos na inicialização
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

            // Agrupa por categoria e soma os valores
            var grupos = _service.Transacoes
                .GroupBy(t => t.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(t => t.Valor) })
                .ToList();

            foreach (var g in grupos)
            {
                PieSeries.Add(new PieSeries<double>
                {
                    Values = new double[] { (double)g.Total },
                    Name = g.Categoria
                });
            }

            // Adiciona item extra apenas para mostrar o total na legenda
            var total = _service.Transacoes.Sum(t => t.Valor);
            PieSeries.Add(new PieSeries<double>
            {
                Values = new double[] { 0 }, // fatia zero, não aparece no gráfico
                Name = $"Total: R$ {total:N2}",
                Fill = null,                // cor transparente
                DataLabelsSize = 0,         // não mostra label no gráfico
                IsVisible = true            // aparece na legenda
            });
        }

        private void AtualizarLine()
        {
            LineSeries.Clear();

            // Converte a data em OADate para eixo X
            var pontos = _service.Transacoes
                .OrderBy(t => t.Data)
                .Select(t => new { X = t.Data.ToOADate(), Y = (double)t.Valor })
                .ToArray();

            LineSeries.Add(new LineSeries<double>
            {
                Values = pontos.Select(p => p.Y).ToArray()
            });
        }
    }
}
