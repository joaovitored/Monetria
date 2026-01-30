using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Monetria.Services;

namespace Monetria.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        // Singleton do serviço de transações compartilhado
        private readonly TransacaoService _transacaoService = AppServices.TransacaoService;

        // Abre e fecha a sidebar
        [ObservableProperty] 
        private bool _isPaneOpen = true;

        // Página atual
        [ObservableProperty] 
        private ViewModelBase _currentPage;

        // Item selecionado no menu
        [ObservableProperty] 
        private ListItemTemplate? _selectedListItem;

        public MainWindowViewModel()
        {
            // Página inicial
            _currentPage = new DashboardPageViewModel(_transacaoService);
        }

        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;

            object? instance;

            // Se for Dashboard, precisa passar o serviço
            if (value.ModelType == typeof(DashboardPageViewModel))
            {
                instance = Activator.CreateInstance(value.ModelType, _transacaoService);
            }
            else if (value.ModelType == typeof(TransacaoPageViewModel))
            {
                instance = Activator.CreateInstance(value.ModelType, _transacaoService);
            }
            else
            {
                // Outras páginas sem parâmetro
                instance = Activator.CreateInstance(value.ModelType);
            }

            if (instance != null)
                CurrentPage = (ViewModelBase)instance;
        }

        public ObservableCollection<ListItemTemplate> ItemsTopo { get; } = new()
        {
            new ListItemTemplate(typeof(DashboardPageViewModel), "Dashboard", "glance_regular"),
            new ListItemTemplate(typeof(TransacaoPageViewModel), "Transações", "money_regular"),
            new ListItemTemplate(typeof(CategoriasPageViewModel), "Categorias", "grid_regular"),
            new ListItemTemplate(typeof(RelatorioPageViewModel), "Relatórios", "book_pulse_regular"),
        };

        public ObservableCollection<ListItemTemplate> ItemsFundo { get; } = new()
        {
            new ListItemTemplate(typeof(ConfiguracoesPageViewModel), "Configurações", "settings_regular"),
            new ListItemTemplate(typeof(SobreMimPageViewModel), "Sobre Mim", "inprivate_account_regular")
        };

        [RelayCommand]
        private void OpenPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }
    }

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
                Application.Current.TryFindResource(iconKey, out var resource) &&
                resource is StreamGeometry geometry)
            {
                ListItemIcon = geometry;
            }
            else
            {
                ListItemIcon = null; // estado válido
            }
        }
    }
}
