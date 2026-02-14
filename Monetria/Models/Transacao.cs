using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Text.Json.Serialization;

namespace Monetria.Models;

public partial class Transacao : ObservableObject
{
    [ObservableProperty] private bool _selecionar;
    [ObservableProperty] private DateTime _data;
    [ObservableProperty] private string _tipo;  // Volta a ser string (mais simples para binding)
    [ObservableProperty] private string _categoria;
    [ObservableProperty] private string _descricao;
    [ObservableProperty] private decimal _valor = 0m;

    [JsonIgnore]
    public IRelayCommand ExcluirCommand { get; set; }  

    [JsonIgnore]
    public string ValorFormatado => $"$ {Valor:N2}";

    
    public Transacao(Action<Transacao> excluir,
        bool selecionar = false,
        DateTime? data = null,
        string tipo = "",  // Parâmetro volta a ser string
        string categoria = "",
        string descricao = "",
        decimal valor = 0m)
    {
        _selecionar = selecionar;
        _data = data ?? DateTime.Now;
        _tipo = tipo;
        _categoria = categoria;
        _descricao = descricao;
        _valor = valor;

        ExcluirCommand = new RelayCommand(() => excluir(this));
    }

    [JsonConstructor]
    public Transacao(DateTime data, string tipo, string categoria, string descricao, decimal valor)  // Construtor JSON volta a usar string
    {
        _data = data;
        _tipo = tipo;
        _categoria = categoria;
        _descricao = descricao;
        _valor = valor;

        ExcluirCommand = null!;
    }
}