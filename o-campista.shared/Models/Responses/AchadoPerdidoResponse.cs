namespace o_campista.shared.Models.Responses;

public class AchadoPerdidoResponse
{
    public long Id { get; set; }
    public long CampingId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? FotoUrl { get; set; }
    public string? LocalGuarda { get; set; }
    public bool Resolvido { get; set; }
    public DateTime CriadoEm { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string? UsuarioFoto { get; set; }
    public bool SouAutor { get; set; }
}
