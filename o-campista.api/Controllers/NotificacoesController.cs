using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/notificacoes")]
[Authorize]
public class NotificacoesController : ControllerBase
{
    private readonly INotificacaoService _notificacaoService;
    private readonly IUsuarioRepository _usuarioRepository;

    public NotificacoesController(INotificacaoService notificacaoService, IUsuarioRepository usuarioRepository)
    {
        _notificacaoService = notificacaoService;
        _usuarioRepository = usuarioRepository;
    }

    // GET api/notificacoes?pagina=1&limite=20
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterNotificacoes([FromQuery] int pagina = 1, [FromQuery] int limite = 20)
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null)
            return Unauthorized();

        var notificacoes = await _notificacaoService.ObterNotificacoesAsync(usuarioId.Value, pagina, limite);
        return Ok(notificacoes);
    }

    // GET api/notificacoes/contagem
    [HttpGet("contagem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterContagem()
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null)
            return Unauthorized();

        var total = await _notificacaoService.ContarNaoLidasAsync(usuarioId.Value);
        return Ok(new { totalNaoLidas = total });
    }

    // PUT api/notificacoes/lidas
    [HttpPut("lidas")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarcarComoLidas()
    {
        var usuarioId = await ObterUsuarioIdAsync();
        if (usuarioId is null)
            return Unauthorized();

        await _notificacaoService.MarcarTodasComoLidasAsync(usuarioId.Value);
        return NoContent();
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
