using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }


        [HttpGet("me/{usuarioId}")]
        public async Task<IActionResult> ObterPerfil(Guid usuarioId)
        {
            var usuario =
                await _usuarioService
                    .ObterPerfilAsync(usuarioId);

            return Ok(usuario);
        }
    }
}
