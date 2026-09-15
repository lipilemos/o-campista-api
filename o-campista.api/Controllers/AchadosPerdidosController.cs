using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class AchadosPerdidosController : ControllerBase
{
    private readonly IAchadoPerdidoService _service;
    private readonly IUsuarioRepository _usuarioRepository;

    public AchadosPerdidosController(
        IAchadoPerdidoService service,
        IUsuarioRepository usuarioRepository)
    {
        _service = service;
        _usuarioRepository = usuarioRepository;
    }

    // GET api/campings/{campingId}/achados-perdidos
    [HttpGet("campings/{campingId:long}/achados-perdidos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Listar(
        long campingId,
        [FromQuery] string? tipo = null,
        [FromQuery] bool incluirResolvidos = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int limite = 20)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            var itens = await _service.ListarAsync(
                usuarioId.Value, campingId, tipo, incluirResolvidos, pagina, limite);
            return Ok(itens);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/campings/{campingId}/achados-perdidos/acesso
    [HttpGet("campings/{campingId:long}/achados-perdidos/acesso")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Acesso(long campingId)
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null)
            return Unauthorized();

        var acesso = await _service.ObterAcessoAsync(usuarioId.Value, campingId);
        return Ok(acesso);
    }

    // POST api/campings/{campingId}/achados-perdidos
    [HttpPost("campings/{campingId:long}/achados-perdidos")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Criar(long campingId, [FromForm] AchadoPerdidoRequest request)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            var item = await _service.CriarAsync(usuarioId.Value, campingId, request);
            return CreatedAtAction(nameof(Listar), new { campingId }, item);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // PATCH api/achados-perdidos/{id}/resolver
    [HttpPatch("achados-perdidos/{id:long}/resolver")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolver(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            await _service.ResolverAsync(id, usuarioId.Value);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // DELETE api/achados-perdidos/{id}
    [HttpDelete("achados-perdidos/{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            await _service.DeletarAsync(id, usuarioId.Value);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // POST api/achados-perdidos/{id}/reivindicar
    [HttpPost("achados-perdidos/{id:long}/reivindicar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Reivindicar(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            var sala = await _service.ReivindicarAsync(id, usuarioId.Value);
            return Ok(sala);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private async Task<Guid?> ObterUsuarioIdAsync()
    {
        var email = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email))
            return null;

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        return usuario?.Id;
    }
}
