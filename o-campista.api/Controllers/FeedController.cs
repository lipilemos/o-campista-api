using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/feed")]
[Authorize]
public class FeedController : ControllerBase
{
    private readonly IFeedService _feedService;
    private readonly IUsuarioRepository _usuarioRepository;

    public FeedController(IFeedService feedService, IUsuarioRepository usuarioRepository)
    {
        _feedService = feedService;
        _usuarioRepository = usuarioRepository;
    }

    // GET api/feed?pagina=1&limite=20
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterFeed([FromQuery] int pagina = 1, [FromQuery] int limite = 20)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            limite = Math.Clamp(limite, 1, 50);
            var feed = await _feedService.ObterFeedAsync(usuarioId.Value, pagina, limite);
            return Ok(feed);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/feed/descobrir?pagina=1&limite=20
    [HttpGet("descobrir")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterDescobrir([FromQuery] int pagina = 1, [FromQuery] int limite = 20)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            limite = Math.Clamp(limite, 1, 50);
            var feed = await _feedService.ObterDescobrirAsync(usuarioId, pagina, limite);
            return Ok(feed);
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
