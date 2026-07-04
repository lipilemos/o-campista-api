using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/ranking")]
[Authorize]
public class RankingController : ControllerBase
{
    private readonly IRankingService _rankingService;
    private readonly IUsuarioRepository _usuarioRepository;

    public RankingController(IRankingService rankingService, IUsuarioRepository usuarioRepository)
    {
        _rankingService = rankingService;
        _usuarioRepository = usuarioRepository;
    }

    // GET api/ranking/global?pagina=1&limite=50
    [HttpGet("global")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterRankingGlobal(
        [FromQuery] int pagina = 1,
        [FromQuery] int limite = 50)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null) return Unauthorized();

            limite = Math.Clamp(limite, 1, 100);
            var ranking = await _rankingService.ObterRankingGlobalAsync(usuarioId.Value, pagina, limite);
            return Ok(ranking);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/ranking/seguidos
    [HttpGet("seguidos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterRankingSeguidos()
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null) return Unauthorized();

            var ranking = await _rankingService.ObterRankingSeguidosAsync(usuarioId.Value);
            return Ok(ranking);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/ranking/campings?pagina=1&limite=20
    [HttpGet("campings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterRankingCampings(
        [FromQuery] int pagina = 1,
        [FromQuery] int limite = 20)
    {
        try
        {
            limite = Math.Clamp(limite, 1, 50);
            var ranking = await _rankingService.ObterRankingCampingsAsync(pagina, limite);
            return Ok(ranking);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
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
