namespace o_campista.shared.Models.Requests
{
    public class CampingAvaliacaoRequest
    {
        public Guid UsuarioId { get; set; }
        public long CampingId { get; set; }
        public long CheckinId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public int XpGanho { get; set; }
    }
}
