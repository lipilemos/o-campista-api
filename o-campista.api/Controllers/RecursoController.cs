using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/recursos")]
[Authorize]
public class RecursoController : ControllerBase
{
    private readonly IRecursoRepository _recursoRepository;

    public RecursoController(IRecursoRepository recursoRepository)
    {
        _recursoRepository = recursoRepository;
    }

    // GET api/recursos
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var recursos = await _recursoRepository.ObterTodosAsync();
        return Ok(recursos.Select(r => new RecursoResponse { Id = r.Id, Nome = r.Nome }));
    }
}
