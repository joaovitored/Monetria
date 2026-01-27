using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Monetria.Models;

namespace Monetria.ViewModels;

public class CategoriasPageViewModel : ViewModelBase
{
    public ObservableCollection<Categoria> Categorias { get; }

    public CategoriasPageViewModel()
    {
        var categorias = new List<Categoria>
        {
            new Categoria("Marcelo", "Verde", "Finança", true)
        };
        Categorias = new ObservableCollection<Categoria>(categorias);
    }
}