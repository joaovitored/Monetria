using System;
using System.Collections.ObjectModel;
using Monetria.Models;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Monetria.ViewModels;

public partial class TransacaoPageViewModel : ViewModelBase
{
    public ObservableCollection<Transacao> Transacoes { get; }

    public TransacaoPageViewModel()
    {
        // Lista inicial de transações
        Transacoes = new ObservableCollection<Transacao>
        {
            new Transacao(
                Excluir,
                selecionar: false,
                data: new DateTime(2023, 10, 20),
                tipo: "Teste",
                categoria: "Categoria",
                descricao: "Descricao inicial",
                valor: 10.50m
            )
        };
    }

    [RelayCommand]
    public void NovaTransacao()
    {
        var nova = new Transacao(
            Excluir,
            selecionar: false,
            data: DateTime.Now,
            tipo: "Tipo",
            categoria: "Categoria",
            descricao: "Nova descrição",
            valor: 0m
        );

        Transacoes.Add(nova);
    }

    [RelayCommand]
    public void Excluir(Transacao transacao)
    {
        if (transacao == null) return;
        Transacoes.Remove(transacao);
    }

    [RelayCommand]
    public async Task ExportarExcelAsync()
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Title = "Salvar arquivo Excel",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Excel Workbook", Extensions = { "xlsx" } }
                },
                DefaultExtension = "xlsx"
            };

            // Obtém referência à janela principal
            var mainWindow = App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (mainWindow == null) return;

            var caminho = await dialog.ShowAsync(mainWindow);
            if (string.IsNullOrWhiteSpace(caminho)) return;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transações");

            // Cabeçalhos
            worksheet.Cell(1, 1).Value = "Data";
            worksheet.Cell(1, 2).Value = "Tipo";
            worksheet.Cell(1, 3).Value = "Categoria";
            worksheet.Cell(1, 4).Value = "Descrição";
            worksheet.Cell(1, 5).Value = "Valor";

            int row = 2;
            foreach (var t in Transacoes)
            {
                worksheet.Cell(row, 1).Value = t.Data;
                worksheet.Cell(row, 2).Value = t.Tipo;
                worksheet.Cell(row, 3).Value = t.Categoria;
                worksheet.Cell(row, 4).Value = t.Descricao;
                worksheet.Cell(row, 5).Value = t.Valor;
                row++;
            }

            // Ajusta a largura das colunas automaticamente
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(caminho);

            Console.WriteLine($"Excel exportado para: {caminho}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao exportar Excel: " + ex.Message);
        }
    }
}
