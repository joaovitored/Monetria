using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Monetria.Models;

public partial class Transacao : ObservableObject
{
    [ObservableProperty] private bool _selecionar;
    [ObservableProperty] private DateTime _data;
    [ObservableProperty] private string _tipo;
    [ObservableProperty] private string _categoria;
    [ObservableProperty] private string _descricao;
    [ObservableProperty] private decimal _valor;

    // Comando de exclusão
    public IRelayCommand ExcluirCommand { get; }

    public Transacao(Action<Transacao> excluir,
        bool selecionar, DateTime data, string tipo, string categoria,
        string descricao, decimal valor)
    {
        _selecionar = selecionar;
        _data = data;
        _tipo = tipo;
        _categoria = categoria;
        _descricao = descricao;
        _valor = valor;

        ExcluirCommand = new RelayCommand(() => excluir(this));
    }
    public string ValorFormatado => $"R$ {_valor:N2}";

}