namespace o_campista.shared.Models.Responses
{
    public class CampingAvaliacaoResponse
    {
        public long Id { get; set; }
        public Guid UsuarioId { get; set; }
        public long CampingId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public DateTime DataCriacao { get; set; }
        public long CheckinId { get; set; }
    }

    public class CampingAvaliacaoComUsuarioResponse : CampingAvaliacaoResponse
    {
        public string UsuarioNome { get; set; } = string.Empty;
        public string UsuarioFoto { get; set; } = string.Empty;
    }
}
