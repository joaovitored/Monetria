using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Monetria.Models;
using Monetria.Services;

namespace Monetria.ViewModels;

public partial class CategoriasPageViewModel : ViewModelBase
{
    private readonly TransacaoService _transacaoService;

    public ObservableCollection<Categoria> Categorias { get; } = new();

    //filtro selecionado
    [ObservableProperty]
    private string _filtroTipo = "All";

    //opções do combobox
    public ObservableCollection<string> TiposFiltro { get; } =
        new() { "All", "Income", "Expenditure" };

    public CategoriasPageViewModel()
    {
        _transacaoService = AppServices.TransacaoService;

        _transacaoService.Transacoes.CollectionChanged += (_, _) => Recarregar();

        Recarregar();
    }

    partial void OnFiltroTipoChanged(string value)
    {
        Recarregar();
    }

    private void Recarregar()
    {
        Categorias.Clear();

        var transacoes = _transacaoService.Transacoes.AsEnumerable();

        if (FiltroTipo != "All")
            transacoes = transacoes.Where(t => t.Type == FiltroTipo);

        var grupos = transacoes
            .GroupBy(t => t.Categories)
            .ToList();

        foreach (var grupo in grupos)
        {
            var tipo = grupo.First().Type;
            var cor = tipo == "Expenditure" ? "Red" : "Green";

            var categoria = new Categoria(grupo.Key, tipo, cor);

            foreach (var transacao in grupo)
                categoria.Transacoes.Add(transacao);

            Categorias.Add(categoria);
        }
    }
}