namespace o_campista.shared.Models.Responses
{
    public class MensagemChatResponse
    {
        public long Id { get; set; }
        public long CampingId { get; set; }
        public Guid UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = null!;
        public string? FotoUsuario { get; set; }
        public string Texto { get; set; } = null!;
        public DateTime DataEnvio { get; set; }
    }
}
