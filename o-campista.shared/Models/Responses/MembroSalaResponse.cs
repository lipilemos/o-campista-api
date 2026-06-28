namespace o_campista.shared.Models.Responses
{
    public class MembroSalaResponse
    {
        public Guid UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = null!;
        public string? FotoUsuario { get; set; }
    }
}
