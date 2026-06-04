using Microsoft.AspNetCore.Mvc;
using o_campista.api.Services;

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
}
