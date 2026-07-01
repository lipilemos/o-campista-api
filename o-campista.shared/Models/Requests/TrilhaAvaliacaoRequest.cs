using Microsoft.AspNetCore.Http;

namespace o_campista.shared.Models.Requests
{
    public class TrilhaAvaliacaoRequest
    {
        public Guid UsuarioId { get; set; }
        public long TrilhaId { get; set; }
        public long CheckinId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public int XpGanho { get; set; }
        public IFormFile? Foto { get; set; }
    }
}
