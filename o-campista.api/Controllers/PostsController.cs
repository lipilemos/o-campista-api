using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IComentarioPostService _comentarioService;
    private readonly IUsuarioRepository _usuarioRepository;

    public PostsController(
        IPostService postService,
        IComentarioPostService comentarioService,
        IUsuarioRepository usuarioRepository)
    {
        _postService = postService;
        _comentarioService = comentarioService;
        _usuarioRepository = usuarioRepository;
    }

    // POST api/posts
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarPost([FromForm] PostViagemRequest request)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            var post = await _postService.CriarPostAsync(usuarioId.Value, request);
            return CreatedAtAction(nameof(CriarPost), new { id = post.Id }, post);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // DELETE api/posts/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletarPost(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            await _postService.DeletarPostAsync(id, usuarioId.Value);
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

    // POST api/posts/{id}/curtir
    [HttpPost("{id}/curtir")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Curtir(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            await _postService.CurtirAsync(id, usuarioId.Value);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // DELETE api/posts/{id}/curtir
    [HttpDelete("{id}/curtir")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Descurtir(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            await _postService.DescurtirAsync(id, usuarioId.Value);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/posts/{id}/curtidas
    [HttpGet("{id}/curtidas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterCurtidas(long id)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            var curtidas = await _postService.ObterCurtidasAsync(id, usuarioId.Value);
            return Ok(curtidas);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // GET api/posts/{postId}/comentarios
    [HttpGet("{postId}/comentarios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterComentarios(long postId, [FromQuery] int pagina = 1, [FromQuery] int limite = 20)
    {
        try
        {
            var comentarios = await _comentarioService.ObterComentariosAsync(postId, pagina, limite);
            return Ok(comentarios);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // POST api/posts/{postId}/comentarios
    [HttpPost("{postId}/comentarios")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarComentario(long postId, [FromBody] ComentarioPostRequest request)
    {
        try
        {
            var usuarioId = await ObterUsuarioIdAsync();
            if (usuarioId is null)
                return Unauthorized();

            var comentario = await _comentarioService.CriarComentarioAsync(postId, usuarioId.Value, request);
            return CreatedAtAction(nameof(ObterComentarios), new { postId }, comentario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
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
