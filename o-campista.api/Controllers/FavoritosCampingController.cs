using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/usuarios/{usuarioId:guid}/favoritos")]
[Authorize]
public class FavoritosCampingController : ControllerBase
{
    private readonly IFavoritoCampingService _favoritoService;
    private readonly IUsuarioRepository _usuarioRepository;

    public FavoritosCampingController(
        IFavoritoCampingService favoritoService,
        IUsuarioRepository usuarioRepository)
    {
        _favoritoService = favoritoService;
        _usuarioRepository = usuarioRepository;
    }

    // GET api/usuarios/{usuarioId}/favoritos
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterFavoritos(Guid usuarioId)
    {
        var requisitanteId = await ObterRequisitanteIdAsync();
        if (requisitanteId != usuarioId) return Forbid();

        var favoritos = await _favoritoService.ObterFavoritosAsync(usuarioId);
        return Ok(favoritos);
    }

    // POST api/usuarios/{usuarioId}/favoritos/{campingId}
    [HttpPost("{campingId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Favoritar(Guid usuarioId, long campingId)
    {
        var requisitanteId = await ObterRequisitanteIdAsync();
        if (requisitanteId != usuarioId) return Forbid();

        await _favoritoService.FavoritarAsync(usuarioId, campingId);
        return NoContent();
    }

    // DELETE api/usuarios/{usuarioId}/favoritos/{campingId}
    [HttpDelete("{campingId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Desfavoritar(Guid usuarioId, long campingId)
    {
        var requisitanteId = await ObterRequisitanteIdAsync();
        if (requisitanteId != usuarioId) return Forbid();

        await _favoritoService.DesfavoritarAsync(usuarioId, campingId);
        return NoContent();
    }

    private async Task<Guid?> ObterRequisitanteIdAsync()
    {
        var email = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email)) return null;
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        return usuario?.Id;
    }
}
