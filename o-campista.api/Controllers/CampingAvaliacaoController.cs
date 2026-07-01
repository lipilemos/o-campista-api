using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.shared.Models.Requests;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/avaliacao")]
    public class CampingAvaliacaoController : ControllerBase
    {
        private readonly ICampingAvaliacaoService _avaliacaoService;

        public CampingAvaliacaoController(ICampingAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CriarAvaliacao([FromForm] CampingAvaliacaoRequest request)
        {
            try
            {
                var avaliacao = await _avaliacaoService.CriarAvaliacaoAsync(request);
                return CreatedAtAction(nameof(CriarAvaliacao), new { id = avaliacao.Id }, avaliacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { mensagem = ex.Message });
            }
        }
               

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterAvaliacao(long id)
        {
            try
            {
                var avaliacao = await _avaliacaoService.ObterPorIdAsync(id);
                if (avaliacao == null)
                {
                    return NotFound(new { mensagem = "Avaliação não encontrada." });
                }
                return Ok(avaliacao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarAvaliacao(long id, [FromBody] CampingAvaliacaoRequest request)
        {
            try
            {
                await _avaliacaoService.AtualizarAvaliacaoAsync(id, request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarAvaliacao(long id)
        {
            try
            {
                await _avaliacaoService.DeletarAvaliacaoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}
