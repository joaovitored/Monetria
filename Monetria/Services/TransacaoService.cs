using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Monetria.Models;

namespace Monetria.Services;

public class TransacaoService
{
    private const string ArquivoJson = "transacoes.json";

    public ObservableCollection<Transacao> Transacoes { get; } = new();

    public TransacaoService()
    {
        Carregar();
        Transacoes.CollectionChanged += (_, _) => Salvar();
    }

    public void AdicionarTransacao(Transacao t) => Transacoes.Add(t);

    public void RemoverTransacao(Transacao t)
    {
        if (t != null)
            Transacoes.Remove(t);
    }

    public void ResetarTudo()
    {
        try
        {
            Transacoes.CollectionChanged -= (_, _) => Salvar();

            Transacoes.Clear();

            if (File.Exists(ArquivoJson))
                File.Delete(ArquivoJson);

            Transacoes.CollectionChanged += (_, _) => Salvar();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao resetar dados: " + ex.Message);
        }
    }

    private void Salvar()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Transacoes, options);
            File.WriteAllText(ArquivoJson, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao salvar JSON: " + ex.Message);
        }
    }

    private void Carregar()
    {
        try
        {
            if (!File.Exists(ArquivoJson)) return;

            var json = File.ReadAllText(ArquivoJson);
            var lista = JsonSerializer.Deserialize<ObservableCollection<Transacao>>(json);

            if (lista == null) return;

            foreach (var t in lista)
            {
                t.ExcluirCommand =
                    new CommunityToolkit.Mvvm.Input.RelayCommand(() => RemoverTransacao(t));

                Transacoes.Add(t);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao carregar JSON: " + ex.Message);
        }
    }
}
