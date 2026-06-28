using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/presentes")]
    public class PresenteController : ControllerBase
    {
        private readonly IPresenteService _service;
        private readonly IUsuarioRepository _usuarioRepository;

        public PresenteController(
            IPresenteService service,
            IUsuarioRepository usuarioRepository)
        {
            _service = service;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetNearby([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                var presentes = await _service.ObterPresentesPorRaioAsync(latitude, longitude);
                return Ok(presentes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao buscar presentes", erro = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromForm] PresenteCreateRequest request)
        {
            try
            {
                var novoPresente = await _service.CriarNovoPresenteAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao criar presente", erro = ex.Message });
            }
        }
        [HttpPost("resgatar")]
        public async Task<IActionResult> Resgatar([FromBody] ResgatarPresenteRequest request)
        {
            try
            {
                await _service.ResgatarAsync(request);

                return Ok(new
                {
                    mensagem = "🎁 Presente resgatado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao resgatar presente", erro = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(long id)
        {
            try
            {
                var email = User.Identity?.Name;
                if (email is null) return Unauthorized();

                var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
                if (usuario is null) return Unauthorized();

                await _service.DeletarAsync(id, usuario.Id);
                return Ok(new { mensagem = "Presente removido com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
