using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/trilhas")]
    public class TrilhaController : ControllerBase
    {
        private readonly IUsuarioTrilhaService _usuarioTrilhaService;

        public TrilhaController(IUsuarioTrilhaService usuarioTrilhaService)
        {
            _usuarioTrilhaService = usuarioTrilhaService;
        }

        [HttpPost("{trilhaId}/concluir")]
        public async Task<IActionResult> Concluir(long trilhaId,[FromQuery] Guid usuarioId)
        {
            await _usuarioTrilhaService.ConcluirTrilhaAsync(usuarioId,trilhaId);

            return Ok(new
            {
                mensagem = "Trilha concluída com sucesso."
            });
        }
    }
}
