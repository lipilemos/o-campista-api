using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("{id}/foto-perfil")]
        [Authorize]
        public async Task<IActionResult> AtualizarFotoPerfil(
            Guid id,
            [FromForm] IFormFile foto)
        {
            if (foto is null || foto.Length == 0)
                return BadRequest("Arquivo de foto é obrigatório.");

            var resultado = await _usuarioService.AtualizarFotoPerfilAsync(id, foto);
            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletarConta(Guid id)
        {
            try
            {
                await _usuarioService.DeletarContaAsync(id);
                return Ok(new { mensagem = "Conta desativada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
