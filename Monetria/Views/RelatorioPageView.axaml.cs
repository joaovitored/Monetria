using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Monetria.ViewModels;

namespace Monetria.Views;

public partial class RelatorioPageView : UserControl
{
    public RelatorioPageView()
    {
        InitializeComponent();
        DataContext = new RelatorioPageViewModel();

    }
}