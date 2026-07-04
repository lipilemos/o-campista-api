namespace o_campista.shared.Models.Responses;

public class PostViagemResponse
{
    public long Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string? UsuarioFoto { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public long? CampingId { get; set; }
    public string? CampingNome { get; set; }
    public long? TrilhaId { get; set; }
    public string? TrilhaNome { get; set; }
    public DateTime CriadoEm { get; set; }
    public int TotalCurtidas { get; set; }
    public bool Curtiu { get; set; }
}
