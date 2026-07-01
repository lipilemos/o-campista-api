using Microsoft.AspNetCore.Mvc;
using o_campista.api.Services;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/mapa")]
public class MapaController : ControllerBase
{
    private readonly IMapaService _mapaService;
    private readonly ICampingAvaliacaoService _avaliacaoService;
    private readonly ICampingFotoRepository _campingFotoRepository;

    public MapaController(IMapaService mapaService, ICampingAvaliacaoService avaliacaoService, ICampingFotoRepository campingFotoRepository)
    {
        _mapaService = mapaService;
        _avaliacaoService = avaliacaoService;
        _campingFotoRepository = campingFotoRepository;
    }

    [HttpGet("campings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterCampings(
        [FromQuery] string? busca = null,
        [FromQuery] string? tipo = null,
        [FromQuery] string[]? recursos = null)
    {
        try
        {
            var resultado = await _mapaService.ObterCampingsMapaAsync(busca, tipo, recursos);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("camping/{idcamping}/avaliacoes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterAvaliacoesCamping(long idcamping)
    {
        try
        {
            var avaliacoes = await _avaliacaoService.ObterAvaliacoesCampingAsync(idcamping);
            return Ok(avaliacoes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    [HttpGet("camping/{campingId}/avaliacoes/{usuarioId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterAvaliacoesPorUsuario(long campingId, Guid usuarioId, [FromQuery] long? checkinId = null)
    {
        try
        {
            var avaliacoes = await _avaliacaoService.ObterAvaliacoesPorUsuarioAsync(campingId, usuarioId, checkinId);
            return Ok(avaliacoes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("camping/{campingId}/fotos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterFotosCamping(long campingId)
    {
        var fotos = await _campingFotoRepository.ObterPorCampingAsync(campingId);
        var response = fotos.Select(f => new CampingFotoResponse
        {
            Id = f.Id,
            Url = f.Url,
            Principal = f.Principal,
            Ordem = f.Ordem,
            CriadoEm = f.CriadoEm
        });
        return Ok(response);
    }
}

