using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace Monetria.Models;

public partial class Categoria : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _type; // Income / Expenditure
    [ObservableProperty] private string _color;

    // Transações ligadas a essa categoria
    public ObservableCollection<Transacao> Transacoes { get; } = new();

    // Total calculado automaticamente
    public decimal Total => Transacoes.Sum(t => t.Value);

    public string TotalFormatado => $"$ {Total:N2}";

    public Categoria(string name, string type, string color)
    {
        _name = name;
        _type = type;
        _color = color;
    }
}