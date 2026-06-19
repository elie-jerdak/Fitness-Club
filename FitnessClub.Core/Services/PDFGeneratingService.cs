using FitnessClub_Test.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class PDFGeneratingService : IPDFGeneratingService
    {
        public byte[] GenerateInvoicePdf(InvoiceDTO invoice)
        {
            var document = new InvoiceDocument(invoice);
            return document.GeneratePdf();
        }

        private class InvoiceDocument : IDocument
        {
            private readonly InvoiceDTO _invoice;

            public InvoiceDocument(InvoiceDTO invoice)
            {
                _invoice = invoice;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                decimal unitPrice = _invoice.member.Type switch
                {
                    "Trial" => 14.99m,
                    "Monthly" => 29.99m,
                    "Yearly" => 324.99m,
                    _ => 0
                };

                int months = Math.Abs(_invoice.member.EndDate.Month - _invoice.member.StartDate.Month);
                decimal subtotal = months * unitPrice;
                decimal tax = subtotal * 0.05m;
                decimal total = subtotal + tax;

                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Text("FitnessClub Invoice")
                        .SemiBold().FontSize(20).FontColor(Colors.Green.Darken2);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Invoice #: {_invoice.InvoiceID}");
                        col.Item().Text($"Date: {_invoice.Date:yyyy-MM-dd}");
                        col.Item().Text($"Customer: {_invoice.User.First_Name} {_invoice.User.Last_Name}");
                        col.Item().Text($"Membership Type: {_invoice.member.Type}");

                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Description").Bold();
                                header.Cell().Text("Months").Bold();
                                header.Cell().Text("Unit Price").Bold();
                                header.Cell().Text("Line Total").Bold();
                            });

                            table.Cell().Text($"{_invoice.member.Type} Membership");
                            table.Cell().Text(months.ToString());
                            table.Cell().Text($"${unitPrice:F2}");
                            table.Cell().Text($"${subtotal:F2}");

                            table.Cell().ColumnSpan(3).AlignRight().Text("Subtotal:");
                            table.Cell().Text($"${subtotal:F2}");

                            table.Cell().ColumnSpan(3).AlignRight().Text("Tax (5%):");
                            table.Cell().Text($"${tax:F2}");

                            table.Cell().ColumnSpan(3).AlignRight().Text("Total:").Bold();
                            table.Cell().Text($"${total:F2}").Bold().FontColor(Colors.Green.Darken2);
                        });
                    });

                    page.Footer().AlignCenter().Text("Thank you for your business!");
                });
            }
        }
    }
}