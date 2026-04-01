using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.DTOs;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<byte[]> GenerateExcelReportAsync(int year, int month);

        Task<bool> UpdateCartAsync(int id, CartUpdateDto dto);
    }
}