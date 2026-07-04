using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/comentarios")]
[Authorize]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioPostService _comentarioService;
    private readonly IUsuarioRepository _usuarioRepository;

    public ComentariosController(
        IComentarioPostService comentarioService,
        IUsuarioRepository usuarioRepository)
    {
        _comentarioService = comentarioService;
        _usuarioRepository = usuarioRepository;
    }

    // DELETE api/comentarios/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletarComentario(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            await _comentarioService.DeletarComentarioAsync(id, usuarioId.Value);
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

    private async Task<Guid?> ObterUsuarioIdAsync()
    {
        var email = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email))
            return null;

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        return usuario?.Id;
    }
}
