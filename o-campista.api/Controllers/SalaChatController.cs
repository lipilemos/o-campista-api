using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    public class SalaChatController : ControllerBase
    {
        private readonly ISalaChatService _salaChatService;
        private readonly IUsuarioRepository _usuarioRepository;

        public SalaChatController(ISalaChatService salaChatService, IUsuarioRepository usuarioRepository)
        {
            _salaChatService = salaChatService;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("salas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterSalas()
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                var salas = await _salaChatService.ObterSalasAsync(usuario.Id);
                return Ok(salas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("salas/{salaId}/mensagens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterMensagens(
            long salaId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                take = Math.Clamp(take, 1, 100);
                var mensagens = await _salaChatService.ObterMensagensAsync(salaId, usuario.Id, skip, take);
                return Ok(mensagens);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("salas/{salaId}/lida")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarcarComoLida(long salaId)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                await _salaChatService.MarcarComoLidaAsync(salaId, usuario.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("salas/{salaId}/pode-enviar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PodeEnviar(long salaId)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                var resultado = await _salaChatService.PodeEnviarAsync(salaId, usuario.Id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("grupos")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CriarGrupo([FromBody] CriarGrupoRequest request)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                var resultado = await _salaChatService.CriarGrupoAsync(usuario.Id, request);
                return Created("", resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("grupos/entrar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EntrarGrupo([FromBody] EntrarGrupoRequest request)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                var sala = await _salaChatService.EntrarGrupoAsync(usuario.Id, request.CodigoConvite);
                return Ok(sala);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("grupos/{salaId}/convite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterConvite(long salaId)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                var codigo = await _salaChatService.ObterConviteAsync(salaId, usuario.Id);
                return Ok(new { codigoConvite = codigo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("grupos/{salaId}/membros")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterMembros(long salaId)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                var membros = await _salaChatService.ObterMembrosAsync(salaId);
                return Ok(membros);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("grupos/{salaId}/sair")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SairDoGrupo(long salaId)
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                await _salaChatService.SairDoGrupoAsync(salaId, usuario.Id);
                return Ok(new { mensagem = "Você saiu do grupo." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("nao-lidas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterNaoLidas()
        {
            try
            {
                var usuario = await ObterUsuarioAutenticado();
                if (usuario is null) return Unauthorized();

                var resultado = await _salaChatService.ObterNaoLidasAsync(usuario.Id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        private async Task<entities.Entities.Usuario?> ObterUsuarioAutenticado()
        {
            var email = User.Identity?.Name;
            if (email is null) return null;
            return await _usuarioRepository.ObterPorEmailAsync(email);
        }
    }
}
