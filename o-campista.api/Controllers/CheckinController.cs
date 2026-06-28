using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.shared.Models.Requests;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/checkin")]
    public class CheckinController : ControllerBase
    {
        private readonly ICheckinService _checkinService;

        public CheckinController(ICheckinService checkinService)
        {
            _checkinService = checkinService;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Checkin([FromBody] CheckinRequest request)
        {
            try
            {
            await _checkinService.RealizarCheckinAsync(
                request);

            return Ok(
                new
                {
                    mensagem = "Check-in realizado"
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });

            }
        }

        [HttpGet("camping/{campingId}/recentes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ContarCheckinsRecentes(long campingId)
        {
            var quantidade = await _checkinService.ContarPessoasUltimas24hAsync(campingId);
            return Ok(new { quantidade });
        }

        [HttpGet("historico/{usuarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterHistorico(Guid usuarioId)
        {
            try
            {
                var historico = await _checkinService.ObtenerHistoricoAsync(usuarioId);
                return Ok(historico);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }
    }
}

