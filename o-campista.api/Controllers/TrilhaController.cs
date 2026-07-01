using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.shared.Models.Requests;

namespace o_campista.api.Controllers
{
    [ApiController]
    [Route("api/trilhas")]
    public class TrilhaController : ControllerBase
    {
        private readonly IUsuarioTrilhaService _usuarioTrilhaService;
        private readonly ITrilhaService _trilhaService;
        private readonly ICheckinService _checkinService;
        private readonly ICampingAvaliacaoService _avaliacaoService;

        public TrilhaController(
            IUsuarioTrilhaService usuarioTrilhaService,
            ITrilhaService trilhaService,
            ICheckinService checkinService,
            ICampingAvaliacaoService avaliacaoService)
        {
            _usuarioTrilhaService = usuarioTrilhaService;
            _trilhaService = trilhaService;
            _checkinService = checkinService;
            _avaliacaoService = avaliacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPorCamping([FromQuery] long campingId, [FromQuery] Guid? usuarioId = null)
        {
            var trilhas = await _trilhaService.ObterPorCampingAsync(campingId, usuarioId);
            return Ok(trilhas);
        }

        [HttpGet("mapa")]
        public async Task<IActionResult> ListarParaMapa()
        {
            try
            {
                var trilhas = await _trilhaService.ObterIndependentesParaMapaAsync();
                return Ok(trilhas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterDetalhe(long id, [FromQuery] Guid? usuarioId = null)
        {
            var trilha = await _trilhaService.ObterDetalheAsync(id, usuarioId);
            if (trilha is null)
                return NotFound(new { mensagem = "Trilha não encontrada." });
            return Ok(trilha);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarTrilhaRequest request)
        {
            try
            {
                var trilha = await _trilhaService.CriarAsync(request);
                return Ok(trilha);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{trilhaId}/concluir")]
        public async Task<IActionResult> Concluir(long trilhaId, [FromQuery] Guid usuarioId)
        {
            try
            {
                await _usuarioTrilhaService.ConcluirTrilhaAsync(usuarioId, trilhaId);
                return Ok(new { mensagem = "Trilha concluída com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{trilhaId}/checkin")]
        public async Task<IActionResult> FazerCheckin(long trilhaId, [FromBody] TrilhaCheckinBody body)
        {
            try
            {
                await _checkinService.RealizarCheckinTrilhaAsync(trilhaId, body.UsuarioId, body.Latitude, body.Longitude);
                return Ok(new { mensagem = "Check-in na trilha realizado! +100 XP 🥾" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{trilhaId}/checkins/recentes")]
        public async Task<IActionResult> ContarCheckinsRecentes(long trilhaId)
        {
            try
            {
                var quantidade = await _checkinService.ContarPessoasTrilhaUltimas24hAsync(trilhaId);
                return Ok(new { quantidade });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{trilhaId}/avaliacoes")]
        public async Task<IActionResult> ListarAvaliacoes(long trilhaId)
        {
            try
            {
                var avaliacoes = await _avaliacaoService.ObterAvaliacoesTrilhaAsync(trilhaId);
                return Ok(avaliacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{trilhaId}/avaliacoes")]
        public async Task<IActionResult> CriarAvaliacao(long trilhaId, [FromForm] TrilhaAvaliacaoRequest request)
        {
            try
            {
                request.TrilhaId = trilhaId;
                var avaliacao = await _avaliacaoService.CriarAvaliacaoTrilhaAsync(request);
                return Ok(avaliacao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }

    public class TrilhaCheckinBody
    {
        public Guid UsuarioId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
