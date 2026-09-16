using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/campings/parceiros")]
[Authorize]
public class CampingParceiroController : ControllerBase
{
    private readonly ICampingParceiroService _service;
    private readonly IUsuarioRepository _usuarioRepository;

    public CampingParceiroController(ICampingParceiroService service, IUsuarioRepository usuarioRepository)
    {
        _service = service;
        _usuarioRepository = usuarioRepository;
    }

    // POST api/campings/parceiros
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CampingParceiroRequest request)
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null) return Unauthorized();

        try
        {
            var criado = await _service.CriarAsync(usuarioId.Value, request);
            return StatusCode(StatusCodes.Status201Created, criado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // POST api/campings/parceiros/{campingId}/reivindicar
    [HttpPost("{campingId:long}/reivindicar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reivindicar(long campingId)
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null) return Unauthorized();

        try
        {
            return Ok(await _service.ReivindicarAsync(usuarioId.Value, campingId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    // GET api/campings/parceiros/meus
    [HttpGet("meus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Meus()
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null) return Unauthorized();

        return Ok(await _service.ObterMeusAsync(usuarioId.Value));
    }

    // GET api/campings/parceiros/proximos?lat=&lng=
    [HttpGet("proximos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Proximos([FromQuery] decimal lat, [FromQuery] decimal lng)
    {
        return Ok(await _service.ObterProximosSemDonoAsync(lat, lng));
    }

    // GET api/campings/parceiros/{campingId}/painel
    [HttpGet("{campingId:long}/painel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Painel(long campingId)
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null) return Unauthorized();

        try
        {
            return Ok(await _service.ObterPainelAsync(usuarioId.Value, campingId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private async Task<Guid?> ObterUsuarioIdAsync()
    {
        var email = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email)) return null;
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        return usuario?.Id;
    }
}
