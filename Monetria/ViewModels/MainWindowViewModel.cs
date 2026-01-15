using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Monetria.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{   // abre e fecha a sidebar
    [ObservableProperty] 
    private bool _isPaneOpen = true;

    //vai mostrar a primeira pagina
    [ObservableProperty] 
    private ViewModelBase _currentPage = new DashboardPageViewModel();
    
    
    
    [ObservableProperty] 
    private ListItemTemplate _selectedListItem;
    
    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if (instance == null) return;
        CurrentPage = (ViewModelBase)instance;


    }
    
    public ObservableCollection<ListItemTemplate> ItemsTopo { get; }= new()
    {
        new ListItemTemplate(typeof(DashboardPageViewModel),"Dashboard","glance_regular"),
        new ListItemTemplate(typeof(TransacaoPageViewModel),"Transações","money_regular"),
        
        //fazer uma pagina nova para esses dois aqui
        new ListItemTemplate(typeof(CategoriasPageViewModel),"Categorias","grid_regular"),
        new ListItemTemplate(typeof(TransacaoPageViewModel),"Relatórios","book_pulse_regular"),

    };

    public ObservableCollection<ListItemTemplate> ItemsFundo { get; } = new()
    {
        new ListItemTemplate(typeof(ConfiguracoesPageViewModel), "Configurações", "settings_regular"),
        new ListItemTemplate(typeof(SobreMimPageViewModel),"Sobre Mim", "inprivate_account_regular")
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
    public StreamGeometry ListItemIcon { get; }
    public ListItemTemplate(Type type,string label,string iconKey)
    {
        ModelType = type;
        Label = label;

        Application.Current.TryFindResource(iconKey, out var resource);
        ListItemIcon = (StreamGeometry)resource;
    }
}