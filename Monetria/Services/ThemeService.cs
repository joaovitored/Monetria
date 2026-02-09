using System.IO;
using Avalonia;
using Avalonia.Styling;

namespace Monetria.Services;

public class ThemeService
{
    private const string ArquivoJson = "tema.json";

    public ThemeVariant TemaAtual { get; private set; } = ThemeVariant.Light;

    public ThemeService()
    {
        Carregar();
        AplicarTema();
    }

    public void DefinirClaro() => DefinirTema(ThemeVariant.Light);
    public void DefinirEscuro() => DefinirTema(ThemeVariant.Dark);
    public void DefinirSystem() => DefinirTema(ThemeVariant.Default);

    private void DefinirTema(ThemeVariant tema)
    {
        TemaAtual = tema;
        AplicarTema();
        Salvar();
    }

    private void AplicarTema()
    {
        if (Application.Current != null)
            Application.Current.RequestedThemeVariant = TemaAtual;
    }

    private void Salvar()
    {
        File.WriteAllText(ArquivoJson, TemaAtual.ToString());
    }

    private void Carregar()
    {
        if (!File.Exists(ArquivoJson)) return;

        var key = File.ReadAllText(ArquivoJson);
        TemaAtual = key switch
        {
            "Dark" => ThemeVariant.Dark,
            "Light" => ThemeVariant.Light,
            "Default" => ThemeVariant.Default,
            _ => ThemeVariant.Light
        };
    }
}