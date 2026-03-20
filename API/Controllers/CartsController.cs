using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {

        private readonly ICartRepository _respository;

        public CartsController(ICartRepository repository)
        {
            _respository = repository;
        }

        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> GetHistory()
        {
            var logs = await _respository.GetAllAsync();

            return Ok(logs);
        }

        [HttpGet]
        [Route("stats")]
        public async Task<IActionResult> GetStats()
        {
            var (large, small) = await _respository.GetActiveCountsAsync();

            return Ok(new
            {
                Large = large,
                Small = small,
                Total = large + large
            });
        }

        [HttpPost]
        [Route("entry")]
        public async Task<IActionResult> RegisterEntry([FromBody] CartLog cart)
        {
            if(await _respository.ExistsActiveAsync(cart.Folio))
            {
                return BadRequest(new
                {
                    message = $"El carro con Folio {cart.Folio} ya está en la planta."
                });
            }

            cart.EntryDate = DateTime.Now;
            cart.Status = CartStatus.InPlant;
            cart.ExitDate = null;

            await _respository.AddAsync(cart);

            return CreatedAtAction(nameof(GetHistory), new { id = cart.Id }, cart);
        }

        [HttpPatch]
        [Route("exit/{folio}")]
        public async Task<IActionResult> RegisterExit(string folio)
        {
            var cart = await _respository.GetActiveByFolioAsync(folio);

            if(cart == null)
            {
                return NotFound(new
                {
                    message = "No se encontró ningún carrito activo con el Folio proporcionado."
                });
            }

            cart.ExitDate = DateTime.Now;
            cart.Status = CartStatus.Completed;

            await _respository.UpdateAsync(cart);

            return NoContent();
        }
    }
}