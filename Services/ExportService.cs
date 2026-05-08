using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace LouietexERP.Services
{
    public interface IExportService
    {
        byte[] GeneratePdf<T>(string title, IEnumerable<T> data, string[] headers, System.Func<T, string[]> rowMapper);
    }

    public class ExportService : IExportService
    {
        public byte[] GeneratePdf<T>(string title, IEnumerable<T> data, string[] headers, System.Func<T, string[]> rowMapper)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Verdana"));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(innerCol =>
                            {
                                innerCol.Item().Text("LouietexERP").FontSize(24).SemiBold().FontColor("#4e73df");
                                innerCol.Item().Text("Enterprise Resource Planning System").FontSize(10).FontColor(Colors.Grey.Medium);
                            });

                            row.RelativeItem().AlignRight().Column(innerCol =>
                            {
                                innerCol.Item().Text(title).FontSize(18).SemiBold();
                                innerCol.Item().Text($"Date: {System.DateTime.Now:MMM dd, yyyy}").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                        });
                        
                        col.Item().PaddingVertical(5).LineHorizontal(1).LineColor("#4e73df");
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                for (int i = 0; i < headers.Length; i++)
                                {
                                    columns.RelativeColumn();
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var h in headers)
                                {
                                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(h).SemiBold();
                                }
                            });

                            foreach (var item in data)
                            {
                                var values = rowMapper(item);
                                foreach (var val in values)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(val ?? "");
                                }
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
