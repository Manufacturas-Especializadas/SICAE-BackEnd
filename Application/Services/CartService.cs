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
    public class CartService
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