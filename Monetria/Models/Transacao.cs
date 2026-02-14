using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Text.Json.Serialization;

namespace Monetria.Models;

public partial class Transacao : ObservableObject
{
    [ObservableProperty] private bool _selecionar;
    [ObservableProperty] private DateTime _date;
    [ObservableProperty] private string _type;  // Tipo da transação
    [ObservableProperty] private string _categories; // Categoria
    [ObservableProperty] private string _description; // Descrição
    [ObservableProperty] private decimal _value = 0m; // Valor da transação

    [JsonIgnore]
    public IRelayCommand ExcluirCommand { get; set; }  

    [JsonIgnore]
    public string ValorFormatado => $"$ {Value:N2}";  // Formatação do valor

    public Transacao(Action<Transacao> excluir,
        bool selecionar = false,
        DateTime? date = null,
        string type = "",  // Tipo de transação
        string categories = "",
        string description = "",
        decimal value = 0m)
    {
        _selecionar = selecionar;
        _date = date ?? DateTime.Now;
        _type = type;
        _categories = categories;
        _description = description;
        _value = value;

        ExcluirCommand = new RelayCommand(() => excluir(this));
    }

    [JsonConstructor]
    public Transacao(DateTime date, string type, string categories, string description, decimal value)
    {
        _date = date;
        _type = type;
        _categories = categories;
        _description = description;
        _value = value;

        ExcluirCommand = null!;
    }
}