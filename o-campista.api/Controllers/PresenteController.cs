using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.shared.Models.Requests;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/presentes")]
    public class PresenteController : ControllerBase
    {
        private readonly IPresenteService _service;

        public PresenteController(IPresenteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetNearby([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                var presentes = await _service.ObterPresentesPorRaioAsync(latitude, longitude);
                return Ok(presentes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao buscar presentes", erro = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromForm] PresenteCreateRequest request)
        {
            try
            {
                var novoPresente = await _service.CriarNovoPresenteAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao criar presente", erro = ex.Message });
            }
        }
        [HttpPost("resgatar")]
        public async Task<IActionResult> Resgatar([FromBody] ResgatarPresenteRequest request)
        {
            try
            {
                await _service.ResgatarAsync(request);

                return Ok(new
                {
                    mensagem = "🎁 Presente resgatado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao resgatar presente", erro = ex.Message });
            }
        }
    }
}
