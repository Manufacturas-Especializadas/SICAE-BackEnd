using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartLog>> GetAllAsync()
        {
            return await _context.CartLogs
                    .Include(c => c.CartType)
                    .OrderBy(c => c.Status == CartStatus.Completed)            
                    .ThenByDescending(c => c.EntryDate)
                    .AsNoTracking()
                    .ToListAsync();
        }

        public async Task<CartLog?> GetActiveByFolioAsync(string folio)
        {
            return await _context.CartLogs
                        .FirstOrDefaultAsync(c => c.Folio.ToUpper() == folio.ToUpper()
                                && c.Status == CartStatus.InPlant);
        }

        public async Task<IEnumerable<(int Year, int Month)>> GetAvailableMonthsAsync()
        {
            return await _context.CartLogs
                .Select(c => new { c.EntryDate.Year, c.EntryDate.Month })
                .Distinct()
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Select(x => ValueTuple.Create(x.Year, x.Month))
                .ToListAsync();
        }

        public async Task<IEnumerable<CartLog>> GetByMonthAsync(int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return await _context.CartLogs
                .Include(c => c.CartType)
                .Where(c =>
                    (c.EntryDate.Year == year && c.EntryDate.Month == month) ||
                    (c.Status == CartStatus.InPlant) ||
                    (c.ExitDate.HasValue && c.ExitDate.Value.Year == year && c.ExitDate.Value.Month == month)
                )
                .OrderBy(c => c.Status == CartStatus.Completed)
                .ThenByDescending(c => c.EntryDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(CartLog cart)
        {
            await _context.CartLogs.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartLog cart)
        {
            _context.CartLogs.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsActiveAsync(string folio)
        {
            return await _context.CartLogs
                    .AnyAsync(c => c.Folio.ToUpper() == folio.ToUpper()
                        && c.Status == CartStatus.InPlant);
        }

        public async Task<(int Large, int Small)> GetActiveCountsAsync()
        {
            var counts = await _context.CartLogs
                    .Where(c => c.Status == CartStatus.InPlant)
                    .GroupBy(c => c.CartTypeId)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync();

            return (
                Large: counts.FirstOrDefault(c => c.Type == (int)CartSize.Large)?.Count ?? 0,
                Small: counts.FirstOrDefault(c => c.Type == (int)CartSize.Small)?.Count ?? 0
            );
        }
    }
}