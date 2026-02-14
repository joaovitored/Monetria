using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Avalonia.Controls;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monetria.Models;
using Monetria.Services;
using Monetria.Enum;
using Avalonia.Platform.Storage;

namespace Monetria.ViewModels;

public partial class TransacaoPageViewModel : ViewModelBase
{
    private readonly TransacaoService _service;

    public ObservableCollection<Transacao> Transacoes => _service.Transacoes;

    // Lista de opções para o ComboBox (hardcoded baseada no enum)
    public List<string> TiposDisponiveis { get; } = new List<string> { TipoTransacao.Income.ToString(), TipoTransacao.Expenditure.ToString() };

    // Propriedade estática para binding direto no XAML (evita problemas de contexto)
    public static List<string> TiposDisponiveisStatic { get; } = new List<string> { TipoTransacao.Income.ToString(), TipoTransacao.Expenditure.ToString() };

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
            var topLevel = App.Current?.ApplicationLifetime
                is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Salvar arquivo Excel",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new FilePickerFileType("Excel Workbook") { Patterns = new[] { "*.xlsx" } }
                },
                DefaultExtension = "xlsx"
            });

            if (file == null) return;

            var caminho = file.Path.LocalPath;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transaction");

            // cabeçalho
            worksheet.Cell(1, 1).Value = "Dat";
            worksheet.Cell(1, 2).Value = "Type";
            worksheet.Cell(1, 3).Value = "Categories";
            worksheet.Cell(1, 4).Value = "Description";
            worksheet.Cell(1, 5).Value = "Value";

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