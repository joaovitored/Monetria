using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monetria.Models;
using Monetria.Services;

namespace Monetria.ViewModels;

public partial class TransacaoPageViewModel : ViewModelBase
{
    private readonly TransacaoService _service;

    public ObservableCollection<Transacao> Transacoes => _service.Transacoes;

    public TransacaoPageViewModel(TransacaoService service)
    {
        _service = service;
    }

    [RelayCommand]
    public void NovaTransacao()
    {
        var t = new Transacao(_service.RemoverTransacao);
        _service.AdicionarTransacao(t);
    }

    [RelayCommand]
    public void Excluir(Transacao t)
    {
        if (t != null) _service.RemoverTransacao(t);
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

            var mainWindow = App.Current.ApplicationLifetime
                is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
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
            foreach (var t in _service.Transacoes)
            {
                worksheet.Cell(row, 1).Value = t.Data;
                worksheet.Cell(row, 2).Value = t.Tipo;
                worksheet.Cell(row, 3).Value = t.Categoria;
                worksheet.Cell(row, 4).Value = t.Descricao;
                worksheet.Cell(row, 5).Value = t.Valor;
                row++;
            }

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
