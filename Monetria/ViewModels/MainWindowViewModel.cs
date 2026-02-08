using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using Monetria.Services;

namespace Monetria.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly TransacaoService _transacaoService = AppServices.TransacaoService;
    private readonly ThemeService _themeService = AppServices.ThemeService;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private ListItemTemplate? _selectedListItem;

    public MainWindowViewModel()
    {
        _ = AppServices.ThemeService;

        _currentPage = new DashboardPageViewModel(_transacaoService);
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;

        object? instance;

        if (value.ModelType == typeof(ConfiguracoesPageViewModel))
        {
            instance = Activator.CreateInstance(
                value.ModelType,
                _transacaoService,
                _themeService);
        }
        else if (
            value.ModelType == typeof(DashboardPageViewModel) ||
            value.ModelType == typeof(TransacaoPageViewModel))
        {
            instance = Activator.CreateInstance(
                value.ModelType,
                _transacaoService);
        }
        else
        {
            instance = Activator.CreateInstance(value.ModelType);
        }

        if (instance is ViewModelBase vm)
            CurrentPage = vm;
    }

    public ObservableCollection<ListItemTemplate> ItemsTopo { get; } = new()
    {
        new(typeof(DashboardPageViewModel), "Dashboard", "glance_regular"),
        new(typeof(TransacaoPageViewModel), "Transações", "money_regular"),
        new(typeof(CategoriasPageViewModel), "Categorias", "grid_regular"),
        new(typeof(RelatorioPageViewModel), "Relatórios", "book_pulse_regular"),
    };

    public ObservableCollection<ListItemTemplate> ItemsFundo { get; } = new()
    {
        new(typeof(ConfiguracoesPageViewModel), "Configurações", "settings_regular"),
        new(typeof(SobreMimPageViewModel), "Sobre Mim", "inprivate_account_regular")
    };

    [RelayCommand]
    private void OpenPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}


// Modelo usado no menu lateral

public class ListItemTemplate
{
    public string Label { get; }
    public Type ModelType { get; }
    public StreamGeometry? ListItemIcon { get; }

    public ListItemTemplate(Type type, string label, string iconKey)
    {
        ModelType = type;
        Label = label;

        if (Application.Current is not null &&
            Application.Current.TryGetResource(iconKey, null, out var resource) &&
            resource is StreamGeometry geometry)
        {
            ListItemIcon = geometry;
        }
    }
}
