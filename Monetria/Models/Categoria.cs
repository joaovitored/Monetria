using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace Monetria.Models;

public partial class Categoria : ObservableObject
{
    [ObservableProperty] private string _nome;
    [ObservableProperty] private string _tipo; // Income / Expenditure
    [ObservableProperty] private string _cor;

    // Transações ligadas a essa categoria
    public ObservableCollection<Transacao> Transacoes { get; } = new();

    // Total calculado automaticamente
    public decimal Total => Transacoes.Sum(t => t.Valor);

    public string TotalFormatado => $"$ {Total:N2}";

    public Categoria(string nome, string tipo, string cor)
    {
        _nome = nome;
        _tipo = tipo;
        _cor = cor;
    }
}