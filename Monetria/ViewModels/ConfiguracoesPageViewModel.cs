using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monetria.Services;
using System;

namespace Monetria.ViewModels;

public partial class ConfiguracoesPageViewModel : ViewModelBase
{
    private readonly TransacaoService _transacaoService;
    private readonly ThemeService _themeService;

    private int _clickCount = 0;

    [ObservableProperty]
    private bool temaEscuro;

    [ObservableProperty]
    private bool temaClaro;

    [ObservableProperty]
    private string message = "Clique 3 vezes para resetar os dados";

    [ObservableProperty]
    private IBrush messageColor = Brushes.Gray;

    public ConfiguracoesPageViewModel(
        TransacaoService transacaoService,
        ThemeService themeService)
    {
        _transacaoService = transacaoService;
        _themeService = themeService;

        //marca os RadioButtons de acordo com o tema carregado
        TemaEscuro = _themeService.TemaAtual.Key?.ToString() == ThemeVariant.Dark.Key?.ToString();
        TemaClaro  = _themeService.TemaAtual.Key?.ToString() == ThemeVariant.Light.Key?.ToString();
    }



    partial void OnTemaEscuroChanged(bool value)
    {
        if (!value) return;

        Console.WriteLine("Tema escuro selecionado");
        _themeService.DefinirEscuro();
    }

    partial void OnTemaClaroChanged(bool value)
    {
        if (!value) return;

        Console.WriteLine("Tema claro selecionado");
        _themeService.DefinirClaro();
    }


    [RelayCommand]
    private async Task ResetarDados()
    {
        _clickCount++;

        if (_clickCount < 3)
        {
            Message = $"Clique {_clickCount}/3 para resetar";
            MessageColor = Brushes.Gray;
            return;
        }

        _transacaoService.ResetarTudo();

        Message = "Dados resetados com sucesso!";
        MessageColor = Brushes.Green;

        _clickCount = 0;

        await Task.Delay(1000);

        Message = "Ready...";
        MessageColor = Brushes.Gray;
    }
}