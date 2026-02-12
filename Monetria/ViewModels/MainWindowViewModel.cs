using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using Monetria.Services;

namespace Monetria.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly TransacaoService _transacaoService = AppServices.TransacaoService;
        private readonly ThemeService _themeService = AppServices.ThemeService;

        [ObservableProperty]
        private bool _isPaneOpen = true;

        [ObservableProperty]
        private ViewModelBase _currentPage;

        // Propriedades separadas para seleção dos menus
        [ObservableProperty]
        private ListItemTemplate? _selectedItemTopo;

        [ObservableProperty]
        private ListItemTemplate? _selectedItemFundo;

        public MainWindowViewModel()
        {
            _ = AppServices.ThemeService;

            _currentPage = new DashboardPageViewModel(_transacaoService);
        }

        partial void OnSelectedItemTopoChanged(ListItemTemplate? value)
        {
            if (value == null) return;

            // Limpa seleção do outro menu
            SelectedItemFundo = null;

            NavigateToPage(value);
        }

        partial void OnSelectedItemFundoChanged(ListItemTemplate? value)
        {
            if (value == null) return;

            // Limpa seleção do outro menu
            SelectedItemTopo = null;

            NavigateToPage(value);
        }

        private void NavigateToPage(ListItemTemplate value)
        {
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
}
