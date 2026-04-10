using iText.Kernel.Pdf;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace RestaurantManagement
{
    public static class InvoicePdfHelper
    {
        public static void CreateInvoicePdf(string filePath, int orderId)
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                    throw new Exception("Order not found.");

                var items = db.OrderItems
                              .Where(oi => oi.OrderId == orderId)
                              .Join(db.Products,
                                    oi => oi.ProductId,
                                    p => p.ProductId,
                                    (oi, p) => new
                                    {
                                        Name = p.Name,
                                        Quantity = oi.Quantity,
                                        UnitPrice = oi.UnitPrice,
                                        Total = oi.Quantity * oi.UnitPrice
                                    })
                              .ToList();

                var payment = db.Payments.FirstOrDefault(p => p.OrderId == orderId);

                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document doc = new Document(pdf))
                {
                    doc.Add(new Paragraph("Orders Payment"));
                    doc.Add(new Paragraph(" "));

                    doc.Add(new Paragraph("Order Info"));
                    doc.Add(new Paragraph("Order ID: " + order.OrderId));
                    doc.Add(new Paragraph("Order Type: " + (order.OrderType ?? "")));
                    doc.Add(new Paragraph("Status: " + (order.Status ?? "")));
                    doc.Add(new Paragraph("Customer Name: " + (order.CustomerName ?? "")));
                    doc.Add(new Paragraph("Customer Phone: " + (order.CustomerPhone ?? "")));
                    doc.Add(new Paragraph("Created At: " +
                        (order.CreatedAt.HasValue ? order.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "")));

                    if (order.TableId != null)
                    {
                        var table = db.RestaurantTables.FirstOrDefault(t => t.TableId == order.TableId.Value);
                        if (table != null)
                        {
                            doc.Add(new Paragraph("Table: " + table.TableNumber));
                        }
                    }

                    doc.Add(new Paragraph("Source: Desktop"));
                    doc.Add(new Paragraph(" "));

                    float[] columnWidths = { 220f, 60f, 100f, 100f };
                    Table tablePdf = new Table(columnWidths);

                    tablePdf.AddHeaderCell("Product");
                    tablePdf.AddHeaderCell("Qty");
                    tablePdf.AddHeaderCell("Unit Price");
                    tablePdf.AddHeaderCell("Total");

                    double grandTotal = 0;

                    foreach (var item in items)
                    {
                        tablePdf.AddCell(item.Name ?? "");
                        tablePdf.AddCell(item.Quantity.ToString());
                        tablePdf.AddCell(Convert.ToDouble(item.UnitPrice).ToString("0.00") + " MAD");
                        tablePdf.AddCell(Convert.ToDouble(item.Total).ToString("0.00") + " MAD");

                        grandTotal += Convert.ToDouble(item.Total);
                    }

                    doc.Add(tablePdf);
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph("Total Amount: " + grandTotal.ToString("0.00") + " MAD"));

                    if (payment != null)
                    {
                        doc.Add(new Paragraph("Payment Method: " + (payment.Method ?? "")));
                        doc.Add(new Paragraph("Paid At: " +
                            (payment.PaidAt.HasValue ? payment.PaidAt.Value.ToString("dd/MM/yyyy HH:mm") : "")));
                    }
                    else
                    {
                        doc.Add(new Paragraph("Payment Method: Not paid yet"));
                    }
                }
            }
        }
    }
}
