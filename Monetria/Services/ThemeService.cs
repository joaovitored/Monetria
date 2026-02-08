using System.IO;
using Avalonia;
using Avalonia.Styling;

namespace Monetria.Services;

public class ThemeService
{
    private const string ArquivoJson = "tema.json";

 
    //tema atual do aplicativo
 
    public ThemeVariant TemaAtual { get; private set; } = ThemeVariant.Light;

    public ThemeService()
    {
        Carregar();
        AplicarTema();
    }

   
    // define o tema claro
    
    public void DefinirClaro() => DefinirTema(ThemeVariant.Light);

 
    //define o tema escuro
  
    public void DefinirEscuro() => DefinirTema(ThemeVariant.Dark);

    
    //aplica o tema na aplicação e salva
    
    private void DefinirTema(ThemeVariant tema)
    {
        TemaAtual = tema;
        AplicarTema();
        Salvar();
    }

  
    //aplica o tema no Application.Current

    private void AplicarTema()
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = TemaAtual;
        }
    }


    //salva apenas a Key do tema no arquivo
   
    private void Salvar()
    {
        // converte pra string
        File.WriteAllText(ArquivoJson, TemaAtual.Key?.ToString() ?? "Light");
    }

   
    //carrega o tema salvo
  
    private void Carregar()
    {
        if (!File.Exists(ArquivoJson))
            return;

        var key = File.ReadAllText(ArquivoJson);

        TemaAtual = key switch
        {
            "Dark" => ThemeVariant.Dark,
            "Light" => ThemeVariant.Light,
            _ => ThemeVariant.Light
        };
    }
}