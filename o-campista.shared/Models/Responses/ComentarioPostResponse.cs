namespace o_campista.shared.Models.Responses;

public class ComentarioPostResponse
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string? UsuarioFoto { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
