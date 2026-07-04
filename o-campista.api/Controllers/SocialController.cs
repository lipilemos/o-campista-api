using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;
    private readonly IUsuarioRepository _usuarioRepository;

    public SocialController(ISocialService socialService, IUsuarioRepository usuarioRepository)
    {
        _socialService = socialService;
        _usuarioRepository = usuarioRepository;
    }

    // GET api/usuarios/{id}/perfil
    [HttpGet("{id}/perfil")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPerfilPublico(Guid id)
    {
        try
        {
            var requisitanteId = await ObterRequisitanteIdAsync();
            var perfil = await _socialService.ObterPerfilPublicoAsync(id, requisitanteId);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // GET api/usuarios/buscar?nome=xxx
    [HttpGet("buscar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BuscarUsuarios([FromQuery] string nome)
    {
        try
        {
            var resultado = await _socialService.BuscarUsuariosAsync(nome);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // POST api/usuarios/{id}/seguir
    [HttpPost("{id}/seguir")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Seguir(Guid id)
    {
        try
        {
            var seguidorId = await ObterRequisitanteIdAsync();
            if (seguidorId is null)
                return Unauthorized();

            await _socialService.SegueAsync(seguidorId.Value, id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // DELETE api/usuarios/{id}/seguir
    [HttpDelete("{id}/seguir")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PararDeSeguir(Guid id)
    {
        try
        {
            var seguidorId = await ObterRequisitanteIdAsync();
            if (seguidorId is null)
                return Unauthorized();

            await _socialService.PararDeSegueAsync(seguidorId.Value, id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/usuarios/{id}/seguidores
    [HttpGet("{id}/seguidores")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterSeguidores(Guid id)
    {
        try
        {
            var lista = await _socialService.ObterSeguidoresAsync(id);
            return Ok(lista);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/usuarios/{id}/seguindo
    [HttpGet("{id}/seguindo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterSeguindo(Guid id)
    {
        try
        {
            var lista = await _socialService.ObterSeguindoAsync(id);
            return Ok(lista);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/usuarios/{id}/privacidade
    [HttpGet("{id}/privacidade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterPrivacidade(Guid id)
    {
        try
        {
            var requisitanteId = await ObterRequisitanteIdAsync();
            if (requisitanteId != id)
                return Forbid();

            var config = await _socialService.ObterPrivacidadeAsync(id);
            return Ok(config);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // PUT api/usuarios/{id}/privacidade
    [HttpPut("{id}/privacidade")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SalvarPrivacidade(Guid id, [FromBody] ConfiguracaoPrivacidadeRequest request)
    {
        try
        {
            var requisitanteId = await ObterRequisitanteIdAsync();
            if (requisitanteId != id)
                return Forbid();

            await _socialService.SalvarPrivacidadeAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/usuarios/sugestoes
    [HttpGet("sugestoes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterSugestoes()
    {
        try
        {
            var requisitanteId = await ObterRequisitanteIdAsync();
            if (requisitanteId is null)
                return Unauthorized();

            var sugestoes = await _socialService.ObterSugestoesAsync(requisitanteId.Value);
            return Ok(sugestoes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private async Task<Guid?> ObterRequisitanteIdAsync()
    {
        var email = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email))
            return null;

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        return usuario?.Id;
    }
}
