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
        Task<CartResponse> RegisterEntry(CartEntryRequest request);

        Task RegisterExit(string folio);

        Task<IEnumerable<CartResponse>> GetDashboard();
    }
}