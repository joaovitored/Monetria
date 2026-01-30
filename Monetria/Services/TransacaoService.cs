using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Monetria.Models;

namespace Monetria.Services;

public class TransacaoService
{
    private const string ArquivoJson = "transacoes.json";

    public ObservableCollection<Transacao> Transacoes { get; } = new ObservableCollection<Transacao>();

    public TransacaoService()
    {
        Carregar();
        Transacoes.CollectionChanged += (s, e) => Salvar();
    }

    public void AdicionarTransacao(Transacao t) => Transacoes.Add(t);

    public void RemoverTransacao(Transacao t)
    {
        if (t != null) Transacoes.Remove(t);
    }

    private void Salvar()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Transacoes, options);
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

            string json = File.ReadAllText(ArquivoJson);
            var lista = JsonSerializer.Deserialize<ObservableCollection<Transacao>>(json);

            if (lista != null)
            {
                foreach (var t in lista)
                {
                    // Inicializa o comando de exclusão após carregar do JSON
                    t.ExcluirCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => RemoverTransacao(t));
                    Transacoes.Add(t);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao carregar JSON: " + ex.Message);
        }
    }
}