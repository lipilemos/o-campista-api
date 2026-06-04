using Microsoft.AspNetCore.Mvc;
using o_campista.api.Services;
using o_campista.shared.Models.Responses;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/mapa")]
public class MapaController : ControllerBase
{
    private readonly IMapaService _mapaService;

    public MapaController(IMapaService mapaService)
    {
        _mapaService = mapaService;
    }

    [HttpGet("campings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterCampings()
    {
        var resultado = await _mapaService.ObterCampingsMapaAsync();

        return Ok(resultado);
    }
    [HttpPost("campings/{campingId}/checkin")]
    public IActionResult Checkin(int campingId)
    {
        return Ok(new CheckinResponse
        {
            Sucesso = true,
            XpGanho = 100,
            NivelAtual = 2,
            Mensagem = "Check-in realizado com sucesso!"
        });
    }
}
