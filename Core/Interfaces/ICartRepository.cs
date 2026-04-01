using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartLog>> GetAllAsync();

        Task<CartLog?> GetActiveByFolioAsync(string folio);

        Task AddAsync(CartLog cart);

        Task UpdateAsync(CartLog cart);

        Task<bool> ExistsActiveAsync(string folio);

        Task<(int Large, int Small)> GetActiveCountsAsync();

        Task<IEnumerable<CartLog>> GetByMonthAsync(int year, int month);
    }
}