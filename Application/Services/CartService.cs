using Application.Interfaces;
using ClosedXML.Excel;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.DTOs;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CartHistoryDto>> GetHistoryAsync()
        {
            var logs = await _repository.GetAllAsync();

            return logs.Select(l => new CartHistoryDto(
                l.Id,
                l.Folio,
                l.CartType?.TypeName ?? "N/A",
                l.EntryDate,
                l.ExitDate,
                l.Status.ToString()
            ));
        }

        public async Task<IEnumerable<CartHistoryDto>> GetMonthlyReportAsync(int year, int month)
        {
            var logs = await _repository.GetByMonthAsync(year, month);

            return logs.Select(l => new CartHistoryDto(
                l.Id,
                l.Folio,
                l.CartType?.TypeName ?? "N/A",
                l.EntryDate,
                l.ExitDate,
                l.Status.ToString()
            ));
        }

        public async Task<IEnumerable<AvailableMonthDto>> GetAvailableMonthsAsync()
        {
            var periods = await _repository.GetAvailableMonthsAsync();
            var culture = new System.Globalization.CultureInfo("es-MX");

            return periods.Select(p => new AvailableMonthDto(
                p.Year,
                p.Month,
                $"{p.Month}{p.Year}",
                $"{culture.DateTimeFormat.GetMonthName(p.Month).ToUpper()} {p.Year}" 
            ));
        }

        public async Task<byte[]> GenerateExcelReportAsync(int year, int month)
        {
            var data = await GetMonthlyReportAsync(year, month);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Carros");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Folio";
                worksheet.Cell(1, 3).Value = "Tipo de Carro";
                worksheet.Cell(1, 4).Value = "Fecha Entrada";
                worksheet.Cell(1, 5).Value = "Fecha Salida";
                worksheet.Cell(1, 6).Value = "Estatus";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                int currentRow = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(currentRow, 1).Value = item.Id;
                    worksheet.Cell(currentRow, 2).Value = item.Folio;
                    worksheet.Cell(currentRow, 3).Value = item.CartTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.EntryDate;

                    if (item.ExitDate.HasValue)
                        worksheet.Cell(currentRow, 5).Value = item.ExitDate.Value;
                    else
                        worksheet.Cell(currentRow, 5).Value = "N/A";

                    worksheet.Cell(currentRow, 6).Value = item.Status;

                    if (item.Status == "InPlant")
                        worksheet.Cell(currentRow, 6).Style.Font.FontColor = XLColor.Orange;

                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<bool> RegisterExitAsync(string folio)
        {
            var cart = await _repository.GetActiveByFolioAsync(folio);

            if (cart == null) return false;

            cart.ExitDate = DateTime.Now;
            cart.Status = CartStatus.Completed;

            await _repository.UpdateAsync(cart);
            return true;
        }
    }
}