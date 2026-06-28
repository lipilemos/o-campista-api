using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUsuarioRepository _usuarioRepository;

        public ChatController(IChatService chatService, IUsuarioRepository usuarioRepository)
        {
            _chatService = chatService;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("{campingId}/mensagens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterMensagens(
            long campingId,
            [FromQuery] int limite = 50,
            [FromQuery] DateTime? antes = null)
        {
            var email = User.Identity?.Name;
            if (email is null) return Unauthorized();

            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario is null) return Unauthorized();

            var temCheckin = await _chatService.TemCheckinValidoAsync(usuario.Id, campingId);
            if (!temCheckin) return Forbid();

            limite = Math.Clamp(limite, 1, 100);
            var mensagens = await _chatService.ObterHistoricoAsync(campingId, limite, antes);
            return Ok(mensagens);
        }
    }
}
