using Application.Interfaces;
using Application.Services;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {

        private readonly ICartRepository _repository;
        private readonly CartService _cartService;

        public CartsController(ICartRepository repository, CartService cartService)
        {
            _repository = repository;
            _cartService = cartService;
        }

        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _cartService.GetHistoryAsync();
            return Ok(history);
        }

        [HttpGet]
        [Route("stats")]
        public async Task<IActionResult> GetStats()
        {
            var (large, small) = await _repository.GetActiveCountsAsync();

            return Ok(new
            {
                Large = large,
                Small = small,
                Total = large + large
            });
        }

        [HttpPost]
        [Route("entry")]
        public async Task<IActionResult> RegisterEntry([FromBody] CartEntryDto dto)
        {
            if (string.IsNullOrEmpty(dto.Folio))
                return BadRequest(new { message = "Folio is required" });

            if (await _repository.ExistsActiveAsync(dto.Folio))
                return BadRequest(new { message = $"Cart {dto.Folio} is already in plant." });

            var cart = new CartLog
            {
                Folio = dto.Folio.ToUpper(),
                CartTypeId = dto.CartTypeId,
                EntryDate = DateTime.Now,
                Status = CartStatus.InPlant,
                ExitDate = null
            };

            await _repository.AddAsync(cart);

            return CreatedAtAction(nameof(GetHistory), new { id = cart.Id }, cart);
        }

        [HttpPatch]
        [Route("exit/{folio}")]
        public async Task<IActionResult> RegisterExit(string folio)
        {
            if (string.IsNullOrWhiteSpace(folio))
            {
                return BadRequest(new { message = "El folio no puede estar vacío." });
            }

            var normalizedFolio = folio.Trim().ToUpper();

            var cart = await _repository.GetActiveByFolioAsync(normalizedFolio);

            if (cart == null)
            {
                return NotFound(new
                {
                    message = $"No se encontró el folio {normalizedFolio} activo en planta o ya marcó salida."
                });
            }

            cart.ExitDate = DateTime.Now;
            cart.Status = CartStatus.Completed;

            try
            {
                await _repository.UpdateAsync(cart);

                return Ok(new
                {
                    message = "Salida registrada con éxito.",
                    folio = cart.Folio,
                    exitDate = cart.ExitDate
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al procesar la salida." });
            }
        }
    }
}