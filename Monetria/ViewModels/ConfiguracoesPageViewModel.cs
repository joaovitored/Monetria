using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monetria.Services;

namespace Monetria.ViewModels;

public partial class ConfiguracoesPageViewModel : ViewModelBase
{
    private readonly TransacaoService _transacaoService;
    private readonly ThemeService _themeService;

    private int _clickCount;

    public IReadOnlyList<AppTheme> AppThemes { get; } =
        new[] { AppTheme.System, AppTheme.Light, AppTheme.Dark };

    [ObservableProperty]
    private AppTheme currentAppTheme;

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

        var tema = themeService.TemaAtual;

        if (tema == ThemeVariant.Dark)
            CurrentAppTheme = AppTheme.Dark;
        else if (tema == ThemeVariant.Light)
            CurrentAppTheme = AppTheme.Light;
        else
            CurrentAppTheme = AppTheme.System;
    }

    partial void OnCurrentAppThemeChanged(AppTheme value)
    {
        switch (value)
        {
            case AppTheme.Dark:
                _themeService.DefinirEscuro();
                break;

            case AppTheme.Light:
                _themeService.DefinirClaro();
                break;

            case AppTheme.System:
                _themeService.DefinirSystem();
                break;
        }
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
